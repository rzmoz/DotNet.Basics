using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public class PathInfo
    {
        private static readonly char _backslash = '\\';
        private static readonly char _slash = '/';

        public PathInfo(string path, params string[] segments)
            : this(path, Sys.IsFolder.Unknown, PathSeparator.Unknown, segments)
        { }

        public PathInfo(string path, Sys.IsFolder isFolder, params string[] segments)
            : this(path, isFolder, PathSeparator.Unknown, segments)
        { }

        public PathInfo(string path, Sys.IsFolder isFolder, PathSeparator pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = isFolder == Sys.IsFolder.Unknown ? DetectIsFolder(path, segments) : isFolder == Sys.IsFolder.True;

            Separator = DetectPathSeparator(pathSeparator, combinedSegments);

            //Clean segments
            Segments = CleanSegments(combinedSegments).ToArray();

            //set rawpath
            RawPath = string.Join(Separator.ToString(), Segments);
            if (IsFolder)
                RawPath = RawPath.EnsureSuffix(Separator);
        }

        public string RawPath { get; }
        public char Separator { get; }
        public bool IsFolder { get; }
        public IReadOnlyCollection<string> Segments { get; }

        //lazy loaded
        public PathDir Parent => GetParent();

        private PathDir GetParent()
        {
            var parentSegments = Segments.Take(Segments.Count - 1).ToArray();
            return parentSegments.Length <= 0 ? null : new PathDir(null, Sys.IsFolder.True, ToPathSeparator(Separator), parentSegments);
        }

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments)
        {
            //to single string
            var joined = string.Join(Separator.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(_backslash, Separator);
            joined = joined.Replace(_slash, Separator);

            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{Separator}]{{2,}}", Separator.ToString(), RegexOptions.None);

            //to segments
            return joined.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool DetectIsFolder(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            return lookingAt.EndsWith(_backslash) || lookingAt.EndsWith(_slash);
        }

        private PathSeparator ToPathSeparator(char separator)
        {
            if (separator == _slash)
                return PathSeparator.Slash;
            if (separator == _backslash)
                return PathSeparator.Backslash;
            return PathSeparator.Unknown;
        }

        private static char DetectPathSeparator(PathSeparator pathSeparator, IEnumerable<string> segments)
        {
            if (segments == null)
                return _backslash;
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
                            return segment[separatorIndex];
                    }
                    return _backslash;
                case PathSeparator.Backslash:
                    return _backslash;
                case PathSeparator.Slash:
                    return _slash;
                default:
                    throw new NotSupportedException($"Separator not supported: {pathSeparator}");
            }
        }
    }
}
