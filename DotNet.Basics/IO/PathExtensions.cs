using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        public const char Slash = '/';
        public const char Backslash = '\\';

        private static readonly string _protocolPattern = @"^([a-zA-Z]+://)";
        private static readonly Regex _protocolRegex = new Regex(_protocolPattern, RegexOptions.Compiled);

        public static PathDelimiter ToPathDelimiter(this char delimiter)
        {
            switch (delimiter)
            {
                case Backslash:
                    return PathDelimiter.Backslash;
                case Slash:
                    return PathDelimiter.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {delimiter}");
            }
        }

        public static bool TryDetectDelimiter(this string path, out PathDelimiter delimiter)
        {
            if (path == null)
            {
                delimiter = PathDelimiter.Backslash;
                return false;
            }

            var slashIndex = path.IndexOf(Slash);
            var backSlashIndex = path.IndexOf(Backslash);

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

        public static char ToChar(this PathDelimiter pathDelimiter)
        {
            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return Backslash;
                case PathDelimiter.Slash:
                    return Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }

        public static bool HasProtocol(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            return _protocolRegex.IsMatch(path);
        }

        public static bool IsProtocol(this string path)
        {
            return path?.EndsWith("://") ?? false;
        }

        public static string[] SplitToSegments(this string[] pathSegments)
        {
            if (pathSegments == null)
                return new string[0];

            var updatedSegments = new List<string>();
            foreach (var pathSegment in pathSegments)
            {
                if (string.IsNullOrWhiteSpace(pathSegment))
                    continue;

                var toBesplit = pathSegment;

                //extract protocol if present
                var protocolMatch = _protocolRegex.Match(toBesplit);
                if (protocolMatch.Success)
                {
                    var capture = protocolMatch.Captures[0].Value;

                    updatedSegments.Add(capture);
                    toBesplit = toBesplit.Substring(capture.Length);//remove protocol
                }

                var splits = toBesplit.Split(new[] { Slash, Backslash },
                    StringSplitOptions.RemoveEmptyEntries);
                updatedSegments.AddRange(splits);
            }

            return updatedSegments.ToArray();
        }
    }
}
