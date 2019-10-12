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
            return path.ToPath(pathType, PathSeparator.Unknown, segments);
        }

        public static PathInfo ToPath(this string path, char pathSeparator, params string[] segments)
        {
            return path.ToPath(PathType.Unknown, pathSeparator, segments);
        }

        public static PathInfo ToPath(this string path, PathType pathType, char pathSeparator, params string[] segments)
        {
            if (pathType == PathType.Unknown)
            {
                var flattened = PathInfo.Flatten(path, segments);
                var cleanedSegments = PathInfo.Flatten(pathType, flattened);
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
                ? path.ToDir(pathSeparator, segments)
                : path.ToFile(pathSeparator, segments) as PathInfo;
        }

        public static PathInfo ToPath(this PathInfo pi, PathType pathType, params string[] segments)
        {
            return pi.RawPath.ToPath(pathType, segments);
        }

        //ToDir
        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return pi.ToDir(pi.Separator, segments);
        }

        public static DirPath ToDir(this PathInfo pi, char pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToDir(pathSeparator, segments);
        }

        public static DirPath ToDir(this string path, params string[] segments)
        {
            return path.ToDir(PathSeparator.Unknown, segments);
        }

        public static DirPath ToDir(this string path, char pathSeparator, params string[] segments)
        {
            return new DirPath(path, pathSeparator, segments);
        }

        //ToFile
        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return pi.ToFile(pi.Separator, segments);
        }

        public static FilePath ToFile(this PathInfo pi, char pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToFile(pathSeparator, segments);
        }

        public static FilePath ToFile(this string path, params string[] segments)
        {
            return path.ToFile(PathSeparator.Unknown, segments);
        }

        public static FilePath ToFile(this string path, char pathSeparator, params string[] segments)
        {
            return new FilePath(path, pathSeparator, segments);
        }

        //Common
        public static T Add<T>(this T pi, params string[] segments) where T : PathInfo
        {
            return pi.RawPath.ToPath(pi.PathType, segments) as T;
        }

        public static bool IsRooted(this PathInfo pi)
        {
            return Path.IsPathRooted(pi.RawPath);
        }
    }
}