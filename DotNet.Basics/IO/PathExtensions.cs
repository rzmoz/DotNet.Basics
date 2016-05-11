using System;
using System.IO;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        public const char SlashDelimiter = '/';
        public const char BackslashDelimiter = '\\';

        public static string ToPath(this string root, PathDelimiter pathDelimiter, params string[] paths)
        {
            var path = Path.Combine(paths);
            path = Path.Combine(root, path);

            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return path.Replace(SlashDelimiter, BackslashDelimiter);
                case PathDelimiter.Slash:
                    return path.Replace(BackslashDelimiter, SlashDelimiter); ;
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
            return root.ToPath(PathDelimiter.Slash);
        }

        /// <summary>
        /// Path delimiters are backslash '\' as usually used in file systems
        /// </summary>
        /// <param name="root"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string ToIoPath(this string root, params string[] paths)
        {
            return root.ToPath(PathDelimiter.Backslash);
        }
    }
}
