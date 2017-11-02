using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public abstract class PathInfo
    {
        private static readonly char[] _separatorDetectors = { PathSeparator.Backslash, PathSeparator.Slash };

        protected PathInfo(string path, params string[] segments)
            : this(path, PathType.Unknown, segments)
        { }

        protected PathInfo(string path, PathType pathType, params string[] segments)
            : this(path, pathType, PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, PathType pathType, char pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = pathType == PathType.Unknown ? DetectIsFolder(path, segments) : pathType == PathType.Folder;

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
        
        public override string ToString()
        {
            return RawPath;
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
    }
}
