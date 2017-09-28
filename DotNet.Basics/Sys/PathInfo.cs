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
        private static readonly char _backslash = '\\';
        private static readonly char _slash = '/';

        protected PathInfo(string path, params string[] segments)
            : this(path, Sys.IsFolder.Unknown, segments)
        { }

        protected PathInfo(string path, Sys.IsFolder isFolder, params string[] segments)
            : this(path, isFolder, PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, Sys.IsFolder isFolder, PathSeparator pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = isFolder == Sys.IsFolder.Unknown ? DetectIsFolder(path, segments) : isFolder == Sys.IsFolder.True;

            PathSeparator = DetectPathSeparator(pathSeparator, combinedSegments);
            var separatorChar = ToChar(PathSeparator);

            //Clean segments
            Segments = CleanSegments(combinedSegments, separatorChar).ToArray();

            //Set rawpath
            RawPath = string.Join(separatorChar.ToString(), Segments);
            RawPath = IsFolder ? RawPath.EnsureSuffix(separatorChar) : RawPath.RemoveSuffix(separatorChar);

            //set name
            Name = Path.GetFileName(RawPath);
        }


        public string RawPath { get; }
        public string Name { get; }
        public bool IsFolder { get; }

        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        public PathSeparator PathSeparator { get; }
        public IReadOnlyCollection<string> Segments;

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments, char separatorChar)
        {
            //to single string
            var joined = string.Join(separatorChar.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(_backslash, separatorChar);
            joined = joined.Replace(_slash, separatorChar);

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

            return lookingAt.EndsWith(_backslash) || lookingAt.EndsWith(_slash);
        }

        private static char ToChar(PathSeparator pathSeparator)
        {
            switch (pathSeparator)
            {
                case PathSeparator.Backslash:
                    return _backslash;
                case PathSeparator.Slash:
                    return _slash;
                default:
                    throw new NotSupportedException($"PathSeparator was {pathSeparator}");
            }
        }
        private static PathSeparator DetectPathSeparator(PathSeparator pathSeparator, IEnumerable<string> segments)
        {
            switch (pathSeparator)
            {
                case PathSeparator.Unknown:
                    foreach (var segment in segments)
                    {
                        if (segment == null)
                            continue;

                        //first separator wins!
                        var separators = new[] { _backslash, _slash };

                        var separatorIndex = segment.IndexOfAny(separators);
                        if (separatorIndex >= 0)
                            return segment[separatorIndex] == _backslash ? PathSeparator.Backslash : PathSeparator.Slash;
                    }
                    return PathSeparator.Backslash;

                default:
                    return pathSeparator;
            }
        }
    }
}
