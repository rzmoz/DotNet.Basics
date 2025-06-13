using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        //ToPath
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return path.ToPath(PathType.Unknown, segments);
        }

        public static PathInfo ToPath(this string path, PathType pathType, params string[] segments)
        {
            if (pathType == PathType.Unknown)
            {
                path = PathInfo.ConformPathSeparator(path ?? string.Empty);
                var root = path.TrimStart().StartsWith(PathInfo.Slash);

                var flattened = PathInfo.Flatten(path, segments);
                var cleanedSegments = PathInfo.Flatten(flattened, root);
                if (string.IsNullOrWhiteSpace(cleanedSegments) == false)
                {
                    var fullName = Path.GetFullPath(cleanedSegments);
                    if (Directory.Exists(fullName))
                        pathType = PathType.Dir;
                    else if (File.Exists(fullName))
                        pathType = PathType.File;
                    else
                        pathType = PathInfo.DetectPathType(path, segments);
                }
            }

            return pathType == PathType.Dir
                ? path.ToDir(segments)
                : path.ToFile(segments);
        }

        public static PathInfo ToPath(this PathInfo pi, PathType pathType, params string[] segments)
        {
            return ToPath(pi.RawPath, pathType, segments);
        }
    }
}