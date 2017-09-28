namespace DotNet.Basics.Sys
{
    public class DirPath : PathInfo
    {
        public DirPath(string path, params string[] segments) : this(path, PathSeparator.Unknown, segments)
        {
        }

        public DirPath(string path, PathSeparator pathSeparator, params string[] segments)
            : base(path, Sys.IsFolder.True, pathSeparator, segments)
        {
        }
    }
}
