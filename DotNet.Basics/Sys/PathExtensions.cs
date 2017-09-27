using System;

namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
        public static PathInfo ToPath(this PathInfo pi, params string[] segments)
        {
            return new PathInfo(pi.RawPath, segments);
        }
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return new PathInfo(path, segments);
        }

        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return new DirPath(pi.RawPath, segments);
        }
        public static DirPath ToDir(this string path, params string[] segments)
        {
            return new DirPath(path, segments);
        }

        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return new FilePath(pi.RawPath, segments);
        }
        public static FilePath ToFile(this string path, params string[] segments)
        {
            return new FilePath(path, segments);
        }

        public static T Add<T>(this T pi, params string[] segments) where T : PathInfo
        {
            switch (pi)
            {
                case DirPath dir:
                    return dir.ToDir(segments) as T;
                case FilePath file:
                    return file.ToFile(segments) as T;
                default:
                    return pi.ToPath(segments) as T;
            }
        }
    }
}
