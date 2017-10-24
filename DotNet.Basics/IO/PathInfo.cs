using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public abstract class PathInfo
    {
        private static readonly char[] _separatorDetectors = { PathSeparator.Backslash, PathSeparator.Slash };

        protected PathInfo(string path, params string[] segments)
            : this(path, IO.IsFolder.Unknown, segments)
        { }

        protected PathInfo(string path, IsFolder isFolder, params string[] segments)
            : this(path, isFolder, PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, IsFolder isFolder, char pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = isFolder == IO.IsFolder.Unknown ? DetectIsFolder(path, segments) : isFolder == IO.IsFolder.True;

            Separator = DetectPathSeparator(pathSeparator, combinedSegments);

            //Clean segments
            Segments = CleanSegments(combinedSegments, Separator).ToArray();

            //Set rawpath
            RawPath = string.Join(Separator.ToString(), Segments);
            RawPath = IsFolder ? RawPath.EnsureSuffix(Separator) : RawPath.RemoveSuffix(Separator);

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Separator));
            NameWoExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
        }

        public string RawPath { get; }
        public string Name { get; }
        public string NameWoExtension { get; }
        public string Extension { get; }
        public bool IsFolder { get; }

        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        public char Separator { get; }
        public IReadOnlyCollection<string> Segments;

        public DirPath Directory()
        {
            return IsFolder ? this.ToDir() : this.Parent;
        }

        public bool DeleteIfExists()
        {
            return DeleteIfExists(30.Seconds());
        }

        public bool DeleteIfExists(TimeSpan timeout)
        {
            if (Exists() == false)
                return true;

            Repeat.Task(() => InternalDeleteIfExists())
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 2.Seconds();
                    o.DontRethrowOnTaskFailedType = typeof(IOException);
                })
                .Until(() => Exists() == false);

            return Exists() == false;
        }

        protected abstract void InternalDeleteIfExists();

        public string FullPath()
        {
#if NETSTANDARD2_0
            return NetStandardIoPath.GetFullPath(RawPath);
#endif
            return NetFrameworkIoPath.GetFullPath(RawPath);
        }

        public bool Exists(IfNotExists ifNotExists = IfNotExists.Mute)
        {
#if NETSTANDARD2_0
            return NetStandardIoPath.Exists(RawPath, ifNotExists);
#endif
            return NetFrameworkIoPath.Exists(RawPath, IsFolder, ifNotExists);
        }

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments, char separatorChar)
        {
            //to single string
            var joined = string.Join(separatorChar.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(PathSeparator.Backslash, separatorChar);
            joined = joined.Replace(PathSeparator.Slash, separatorChar);

            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{separatorChar}]{{2,}}", separatorChar.ToString(), RegexOptions.None);

            //to segments
            return joined.Split(new[] { separatorChar }, StringSplitOptions.RemoveEmptyEntries).Where(seg => String.IsNullOrWhiteSpace(seg) == false);
        }

        public static bool DetectIsFolder(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return false;

            return lookingAt.EndsWith(PathSeparator.Backslash.ToString()) || lookingAt.EndsWith(PathSeparator.Slash.ToString());
        }

        private static char DetectPathSeparator(char pathSeparator, IEnumerable<string> segments)
        {
            if (_separatorDetectors.Contains(pathSeparator))
                return pathSeparator;

            if (pathSeparator == PathSeparator.Unknown)
                //auto detect supported separators
                foreach (var segment in segments)
                {
                    if (segment == null)
                        continue;
                    //first separator wins!
                    var separatorIndex = segment.IndexOfAny(_separatorDetectors);
                    if (separatorIndex >= 0)
                        return segment[separatorIndex];
                }

            return PathSeparator.Backslash;//default
        }
    }
}
