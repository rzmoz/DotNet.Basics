using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNet.Basics.IO
{
    public static class PathInfoExtensions
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

        public static PathInfo ToPath(this string path, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var allSegments = new[] { path }.Concat(segments).ToArray();
            
            var pathSegmentWithFolderToken = segments.Length > 0 ? segments.Last() : path;
            var detectedIsFolder = pathSegmentWithFolderToken.IsFolder();

            var asPath = Create(allSegments, detectedIsFolder);
            asPath.Delimiter = asPath.IsUri ? PathDelimiter.Slash : PathDelimiter.Backslash;

            if (asPath.IsUri)
                return asPath;
            
            foreach (var segment in allSegments)
            {
                PathDelimiter delimiter;
                if (TryDetectDelimiter(segment, out delimiter))
                {
                    asPath.Delimiter = delimiter;
                    break;
                }
            }

            var asFolder = Create(allSegments, true);
            if (SystemIoPath.Exists(asFolder.FullName, true))
                return asFolder;

            if (SystemIoPath.Exists(asFolder.FullName, false))
                return Create(allSegments, false);
            return asPath;
        }

        public static PathInfo ToPath(this string path, bool isFolder, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            var asPath = Create(new[] { path }, isFolder).Add(segments);
            PathDelimiter delimiter;
            if (TryDetectDelimiter(path, out delimiter))
                asPath.Delimiter = delimiter;
            return asPath;
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

        private static PathInfo Create(string[] pathSegments, bool isFolder)
        {
            if (isFolder)
                return new DirPath(pathSegments);
            return new FilePath(pathSegments);
        }

        private static bool TryDetectDelimiter(string path, out PathDelimiter delimiter)
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
    }
}
