using System;

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

        public static Path ToPath(this string root, params string[] paths)
        {
            var path = new Path(root).Add(paths);
            return path;
        }
        public static Path ToPath(this string root, bool isFolder, params string[] paths)
        {
            var path = new Path(root).Add(paths);
            return path;
        }
        public static Path ToPath(this string root, PathDelimiter delimiter, params string[] paths)
        {
            var path = new Path(root).Add(paths);
            path.Delimiter = delimiter;
            return path;
        }
    }
}
