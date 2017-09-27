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

        private readonly char _separator;
        private readonly string[] _segments;

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

            _separator = DetectPathSeparator(pathSeparator, combinedSegments);

            //Clean segments
            _segments = CleanSegments(combinedSegments).ToArray();

            //set rawpath
            RawPath = string.Join(_separator.ToString(), _segments);
            if (IsFolder)
                RawPath = RawPath.EnsureSuffix(_separator);
        }

        public string RawPath { get; }
        public DirPath Directory => IsFolder ? this.ToDir() : Parent;
        public bool IsFolder { get; }
        public DirPath Parent => GetParent();//lazy loaded

        private DirPath GetParent()
        {
            var parentSegments = _segments.Take(_segments.Length - 1).ToArray();
            return parentSegments.Length <= 0 ? null : new DirPath(null, Sys.IsFolder.True, ToPathSeparator(_separator), parentSegments);
        }

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments)
        {
            //to single string
            var joined = string.Join(_separator.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(_backslash, _separator);
            joined = joined.Replace(_slash, _separator);

            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{_separator}]{{2,}}", _separator.ToString(), RegexOptions.None);

            //to segments
            return joined.Split(new[] { _separator }, StringSplitOptions.RemoveEmptyEntries);
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
