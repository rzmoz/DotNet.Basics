namespace DotNet.Basics.Sys
{
    public class DirPath(string path, params string[] segments) : PathInfo(path, PathType.Dir, segments)
    {
        public static explicit operator DirPath(string s)
        {
            return new DirPath(s);
        }
    }
}