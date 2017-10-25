namespace DotNet.Basics.Sys
{
    public class FilePath : PathInfo
    {
        public FilePath(string path, params string[] segments)
            : this(path, PathSeparator.Unknown, segments)
        {
        }

        public FilePath(string path, char pathSeparator, params string[] segments)
            : base(path, Sys.IsFolder.False, pathSeparator, segments)
        {
        }
    }
}
