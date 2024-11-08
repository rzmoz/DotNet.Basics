namespace DotNet.Basics.Sys
{
    public class FilePath(string path, params string[] segments)
        : PathInfo(path, PathType.File, segments)
    {
        public static explicit operator FilePath(string s)
        {
            return new FilePath(s);
        }
    }
}