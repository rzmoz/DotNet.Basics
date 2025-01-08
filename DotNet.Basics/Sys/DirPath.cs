namespace DotNet.Basics.Sys
{
    public class DirPath(string path, params string[] segments) : PathInfo(path, PathType.Dir, segments)
    {
        public static implicit operator DirPath(string s)
        {
            return new DirPath(s);
        }
        public static implicit operator string(DirPath p)
        {
            return p.RawPath;
        }
    }
}