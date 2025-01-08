namespace DotNet.Basics.Sys
{
    public class FilePath(string path, params string[] segments)
        : PathInfo(path, PathType.File, segments)
    {
        public static implicit operator FilePath(string s)
        {
            return new FilePath(s);
        }
        public static implicit operator string(FilePath p)
        {
            return p.RawPath;
        }
    }
}