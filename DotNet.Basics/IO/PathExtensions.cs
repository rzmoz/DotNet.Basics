using System;
using System.IO;

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

        public static string ToPath(this string root, PathDelimiter pathDelimiter, params string[] paths)
        {
            var path = Path.Combine(paths);
            path = Path.Combine(root, path.TrimStart(_slashDelimiter).TrimStart(_backslashDelimiter).TrimStart(_slashDelimiter));

            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return path.Replace(_slashDelimiter, _backslashDelimiter);
                case PathDelimiter.Slash:
                    return path.Replace(_backslashDelimiter, _slashDelimiter); ;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }

        /// <summary>
        /// Path delimiters are slash '/' as usually used in Uris
        /// </summary>
        /// <param name="root"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string ToUriPath(this string root, params string[] paths)
        {
            return root.ToPath(PathDelimiter.Slash, paths);
        }

        /// <summary>
        /// Path delimiters are backslash '\' as usually used in file systems
        /// </summary>
        /// <param name="root"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string ToIoPath(this string root, params string[] paths)
        {
            return root.ToPath(PathDelimiter.Backslash, paths);
        }
    }
}
