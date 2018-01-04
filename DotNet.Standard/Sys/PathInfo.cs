using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Standard.Collections;

namespace DotNet.Standard.Sys
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

            Separator = pathSeparator != PathSeparator.Unknown ? pathSeparator : DetectPathSeparator(path, segments);

            //Clean segments
            Segments = Tokenize(path, segments);

            PathType = pathType == PathType.Unknown ? DetectPathType(path, segments) : pathType;

            //Set rawpath
            RawPath = Flatten(null, PathType, Segments.ToArray());
            RawPath = OverridePathSeparator(RawPath, Separator);

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Separator));
            NameWoExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
        }

        public string RawPath { get; }
        public string Name { get; }
        public string NameWoExtension { get; }
        public string Extension { get; }
        public PathType PathType { get; }

        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        public char Separator { get; }
        public IReadOnlyCollection<string> Segments;

        public static IReadOnlyCollection<string> Tokenize(string path, params string[] segments)
        {
            var tokens = new List<string>();
            tokens.AddRange(Tokenize(path));
            foreach (var segment in segments)
                tokens.AddRange(Tokenize(segment));
            return tokens;
        }

        private static IEnumerable<string> Tokenize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Enumerable.Empty<string>();

            path = OverridePathSeparator(path, PathSeparator.Backslash);

            return path.Split(new[] { PathSeparator.Backslash }, StringSplitOptions.RemoveEmptyEntries)
                .Where(seg => String.IsNullOrWhiteSpace(seg) == false);
        }

        private static string OverridePathSeparator(string path, char separator)
        {
            //conform separators
            path = path.Replace(PathSeparator.Slash, separator);
            path = path.Replace(PathSeparator.Backslash, separator);
            path = path.Replace(PathSeparator.Unknown, separator);

            return path;
        }

        public static string Flatten(string path, PathType pathType, params string[] segments)
        {
            var tokenized = Tokenize(path, segments);
            var separator = DetectPathSeparator(path, segments);
            var flattened = string.Join(separator.ToString(), tokenized);
            if (pathType == PathType.Dir)
                flattened = flattened.EnsureSuffix(separator);
            return flattened;
        }

        public static PathType DetectPathType(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return PathType.Unknown;

            if (lookingAt.EndsWith(PathSeparator.Backslash.ToString()) || lookingAt.EndsWith(PathSeparator.Slash.ToString()))
                return PathType.Dir;
            return PathType.File;
        }

        private static char DetectPathSeparator(string path, IEnumerable<string> segments)
        {
            //auto detect supported separators
            foreach (var segment in path.ToEnumerable(segments))
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

        public override string ToString()
        {
            return RawPath;
        }
    }
}
