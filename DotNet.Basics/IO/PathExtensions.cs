using System;
using System.Collections.Generic;
using System.IO;
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
            var path = Path.Combine(paths);
            return Path.Combine(root, path);
        }

        public static IList<string> ToPathTokens(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return new List<string>();

            return path.Split(new char[] { _slashDelimiter, _backslashDelimiter }, StringSplitOptions.None);
        }
    }
}
