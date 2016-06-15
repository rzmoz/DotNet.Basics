using System;
using System.Collections.Generic;

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

        public static Path Add(this Path path, params string[] pathTokens)
        {
            var newPath = path;
            foreach (var pathToken in pathTokens)
            {
                newPath = path.Add(pathToken);
            }
            return newPath;
        }
        public static FilePath ToFilePath(this string path, params string[] pathTokens)
        {
            return (FilePath)new FilePath(path).Add(pathTokens);
        }
        public static FilePath ToFilePath(this Path path)
        {
            return new FilePath(path.PathTokens, path.Delimiter);
        }
        public static DirPath ToDirPath(this string path, params string[] pathTokens)
        {
            return (DirPath)new DirPath(path).Add(pathTokens);
        }
        public static DirPath ToDirPath(this Path path)
        {
            return new DirPath(path.PathTokens, path.Delimiter);
        }
        
        public static Path ToPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var isFolder = DetectIsFolder(path);
            return ToPath(path, isFolder);
        }
        public static Path ToPath(this string path, bool isFolder)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var delimiter = DetectDelimiter(null, path);

            if (isFolder)
                return new DirPath(new[] { path }, delimiter);
            else
                return new FilePath(new[] { path }, delimiter);
        }

        private static bool DetectIsFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return path.EndsWith(_slashDelimiter.ToString()) || path.EndsWith(_backslashDelimiter.ToString());
        }
        private static PathDelimiter DetectDelimiter(string protocol, string path)
        {
            PathDelimiter delimiter;
            if (TryDetectDelimiter(protocol, out delimiter) == false)
                TryDetectDelimiter(path, out delimiter);
            return delimiter;
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
        private static Path Add(this Path path, string pathToken)
        {
            if (string.IsNullOrWhiteSpace(pathToken))
                return path;

            var updatedSegments = path.PathTokens == null ? new List<string>() : new List<string>(path.PathTokens);
            updatedSegments.AddRange(pathToken.Split(new[] { _slashDelimiter, _backslashDelimiter }, StringSplitOptions.RemoveEmptyEntries));
            
            if (path.IsFolder)
                return new DirPath(updatedSegments.ToArray(), path.Delimiter);
            return new FilePath(updatedSegments.ToArray(), path.Delimiter);
        }
    }
}
