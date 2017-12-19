namespace DotNet.Basics.Sys
{
    public class DirPath : PathInfo
    {
        public DirPath(string path, params string[] segments) : this(path, PathSeparator.Unknown, segments)
        {
        }

        public DirPath(string path, char pathSeparator, params string[] segments)
            : base(path, PathType.Dir, pathSeparator, segments)
        {
        }        
    }
}
