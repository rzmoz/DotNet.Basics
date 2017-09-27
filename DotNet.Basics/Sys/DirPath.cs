namespace DotNet.Basics.Sys
{
    public class DirPath : PathInfo
    {
        public DirPath(string path, params string[] segments) : base(path, segments)
        {
        }

        public DirPath(string path, IsFolder isFolder, params string[] segments) : base(path, isFolder, segments)
        {
        }

        public DirPath(string path, IsFolder isFolder, PathSeparator pathSeparator, params string[] segments) : base(path, isFolder, pathSeparator, segments)
        {
        }
    }
}
