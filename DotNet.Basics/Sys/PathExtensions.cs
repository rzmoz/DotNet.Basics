namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
        //ToDir
        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return ToDir(pi.RawPath, segments);
        }

        public static DirPath ToDir(this string path, params string[] segments)
        {
            return new DirPath(path, segments);
        }

        //ToFile
        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return ToFile(pi.RawPath, segments);
        }

        public static FilePath ToFile(this string path, params string[] segments)
        {
            return new FilePath(path, segments);
        }
    }
}