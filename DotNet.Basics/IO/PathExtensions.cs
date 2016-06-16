using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        private const char _slashDelimiter = '/';
        private const char _backslashDelimiter = '\\';

        public static PathDelimiter ToPathDelimiter(this char delimiter)
        {
            switch (delimiter)
            {
                case _backslashDelimiter:
                    return PathDelimiter.Backslash;
                case _slashDelimiter:
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
                    return _backslashDelimiter;
                case PathDelimiter.Slash:
                    return _slashDelimiter;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }
        
        

        public static Path ToPath(this string path, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var isFolder = DetectIsFolder(segments.Length > 0 ? segments.Last() : path);

            var allSegments = new[] { path }.Concat(segments).ToArray();

            PathDelimiter delimiter = PathDelimiter.Backslash;
            foreach (var segment in allSegments)
            {
                if (TryDetectDelimiter(segment, out delimiter))
                    break;
            }

            return Create(allSegments, isFolder, delimiter);
        }
        public static Path ToPath(this string path, bool isFolder)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            PathDelimiter delimiter;
            TryDetectDelimiter(path, out delimiter);
            return Create(new[] { path }, isFolder, delimiter);
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

                var split = pathSegment.Split(new[] { _slashDelimiter, _backslashDelimiter },
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

        private static bool DetectIsFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return path.EndsWith(_slashDelimiter.ToString()) || path.EndsWith(_backslashDelimiter.ToString());
        }

        private static bool TryDetectDelimiter(string path, out PathDelimiter delimiter)
        {
            if (path == null)
            {
                delimiter = PathDelimiter.Backslash;
                return false;
            }

            var slashIndex = path.IndexOf(_slashDelimiter);
            var backSlashIndex = path.IndexOf(_backslashDelimiter);

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
