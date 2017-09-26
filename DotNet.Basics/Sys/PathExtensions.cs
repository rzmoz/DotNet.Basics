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

        public static PathDir ToDir(this PathInfo pi, params string[] segments)
        {
            return new PathDir(pi.RawPath, segments);
        }
        public static PathDir ToDir(this string path, params string[] segments)
        {
            return new PathDir(path, segments);
        }

        public static PathFile ToFile(this PathInfo pi, params string[] segments)
        {
            return new PathFile(pi.RawPath, segments);
        }
        public static PathFile ToFile(this string path, params string[] segments)
        {
            return new PathFile(path, segments);
        }

        public static T Add<T>(this T pi, params string[] segments) where T : PathInfo
        {
            switch (pi)
            {
                case PathDir dir:
                    return dir.ToDir(segments) as T;
                case PathFile file:
                    return file.ToFile(segments) as T;
                default:
                    return pi.ToPath(segments) as T;
            }
        }
    }
}
