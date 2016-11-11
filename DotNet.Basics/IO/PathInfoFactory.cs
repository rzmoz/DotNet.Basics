using System.Linq;

namespace DotNet.Basics.IO
{
    public static class PathInfoFactory
    {
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
                if (segment.TryDetectDelimiter(out delimiter))
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
            if (path.TryDetectDelimiter(out delimiter))
                asPath.Delimiter = delimiter;
            return asPath;
        }

        private static PathInfo Create(string[] pathSegments, bool isFolder)
        {
            if (isFolder)
                return new DirPath(pathSegments);
            return new FilePath(pathSegments);
        }
    }
}
