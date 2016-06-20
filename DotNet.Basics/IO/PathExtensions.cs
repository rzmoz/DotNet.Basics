using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        public static PathDelimiter ToPathDelimiter(this char delimiter)
        {
            switch (delimiter)
            {
                case PathDelimiterAsChar.Backslash:
                    return PathDelimiter.Backslash;
                case PathDelimiterAsChar.Slash:
                    return PathDelimiter.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {delimiter}");
            }
        }

        public static char ToChar(this PathDelimiter pathDelimiter)
        {
            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return PathDelimiterAsChar.Backslash;
                case PathDelimiter.Slash:
                    return PathDelimiterAsChar.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }

        public static Path ToPath(this string path, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var allSegments = new[] { path }.Concat(segments).ToArray();

            PathDelimiter delimiter = PathDelimiter.Backslash;
            foreach (var segment in allSegments)
            {
                if (TryDetectDelimiter(segment, out delimiter))
                    break;
            }

            var asFolder = Create(allSegments, true, delimiter);
            if (SystemIoPath.Exists(asFolder.FullName, true))
                return asFolder;

            if (SystemIoPath.Exists(asFolder.FullName, false))
                return Create(allSegments, false, delimiter);

            var pathSegmentWithFolderToken = segments.Length > 0 ? segments.Last() : path;
            var isFolder = pathSegmentWithFolderToken.IsFolder();
            return Create(allSegments, isFolder, delimiter);
        }
        public static Path ToPath(this string path, bool isFolder, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            PathDelimiter delimiter;
            TryDetectDelimiter(path, out delimiter);
            return Create(new[] { path }, isFolder, delimiter).Add(segments);
        }

        public static string[] SplitSegments(this string[] pathSegments)
        {
            if (pathSegments == null)
                return new string[0];

            var updatedSegments = new List<string>();
            foreach (var pathSegment in pathSegments)
            {
                if (string.IsNullOrWhiteSpace(pathSegment))
                    continue;

                var split = pathSegment.Split(new[] { PathDelimiterAsChar.Slash, PathDelimiterAsChar.Backslash },
                    StringSplitOptions.RemoveEmptyEntries);
                updatedSegments.AddRange(split);
            }

            return updatedSegments.ToArray();
        }

        private static Path Create(string[] pathSegments, bool isFolder, PathDelimiter delimiter)
        {
            if (isFolder)
                return new DirPath(pathSegments, delimiter);
            return new FilePath(pathSegments, delimiter);
        }



        private static bool TryDetectDelimiter(string path, out PathDelimiter delimiter)
        {
            if (path == null)
            {
                delimiter = PathDelimiter.Backslash;
                return false;
            }

            var slashIndex = path.IndexOf(PathDelimiterAsChar.Slash);
            var backSlashIndex = path.IndexOf(PathDelimiterAsChar.Backslash);

            if (slashIndex < 0)
            {
                delimiter = PathDelimiter.Backslash;
                return true;
            }

            if (backSlashIndex < 0)
            {
                delimiter = PathDelimiter.Slash;
                return true;
            }

            //first occurence decides
            delimiter = slashIndex < backSlashIndex ? PathDelimiter.Slash : PathDelimiter.Backslash;
            return true;
        }
    }
}
