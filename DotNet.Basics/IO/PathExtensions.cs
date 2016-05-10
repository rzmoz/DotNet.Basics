using System;
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

        public static string ToPath(this string root, PathDelimiter pathDelimiter = PathDelimiter.Backslash, params string[] paths)
        {
            return root.ToPath(paths).WithDelimiter(pathDelimiter);
        }
        public static string ToPath(this string root, params string[] paths)
        {
            var path = Path.Combine(paths.Select(TrimPath).ToArray());
            return Path.Combine(TrimPath(root), path);
        }

        private static string TrimPath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return string.Empty;
            return rawPath.Trim(_slashDelimiter).Trim(_backslashDelimiter).Trim(_slashDelimiter);
        }
    }
}
