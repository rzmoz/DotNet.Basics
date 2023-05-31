namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
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
            return string.IsNullOrEmpty(path) ? null : new DirPath(path, pathSeparator, segments);
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
            return string.IsNullOrEmpty(path) ? null : new FilePath(path, pathSeparator, segments);
        }
    }
}