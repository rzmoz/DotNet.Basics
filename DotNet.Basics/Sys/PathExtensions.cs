namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
        //ToDir
        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return pi.RawPath.ToDir(segments);
        }

        public static DirPath ToDir(this string path, params string[] segments)
        {
            return string.IsNullOrEmpty(path) ? null : new DirPath(path, segments);
        }

        //ToFile
        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return pi.RawPath.ToFile(segments);
        }
        
        public static FilePath ToFile(this string path, params string[] segments)
        {
            return string.IsNullOrEmpty(path) ? null : new FilePath(path, segments);
        }
    }
}