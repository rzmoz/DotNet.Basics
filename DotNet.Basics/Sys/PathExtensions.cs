using System;

namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
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
                    throw new NotSupportedException($"Type not supported. Was: {typeof(T)}");
            }
        }
    }
}
