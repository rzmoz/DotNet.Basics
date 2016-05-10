using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        private const char _slashDelimiter = '/';
        private const char _backslashDelimiter = '\\';

        public static string WithDelimiter(this string path, PathDelimiter pathDelimiter = PathDelimiter.Backslash)
        {
            if (path == null)
                return null;
            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return path.Replace(_slashDelimiter, _backslashDelimiter);
                case PathDelimiter.Slash:
                    return path.Replace(_backslashDelimiter, _slashDelimiter);
                default:
                    throw new NotSupportedException($"Path Delimiter not supported:{pathDelimiter}");
            }
        }

        public static string ToPath(this IEnumerable<string> paths)
        {
            var pathsList = paths.Select(CleanPath).ToArray();
            return Path.Combine(pathsList);
        }

        public static string ToPath(this FileSystemInfo root, params string[] paths)
        {
            return root.ToString().ToPath(paths);
        }

        public static string ToPath(this string root, params string[] paths)
        {
            return ToPath(new[] { root }, paths);
        }

        public static string ToPath(this IEnumerable<string> root, params string[] paths)
        {
            var allPaths = new List<string>();
            if (root != null)
                allPaths.AddRange(root);
            if (paths.Any())
                allPaths.AddRange(paths);
            return ToPath(allPaths.ToArray());
        }

        private static string CleanPath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return string.Empty;
            return rawPath.Trim(_slashDelimiter).Trim(_backslashDelimiter).Trim(_slashDelimiter);
        }
    }
}
