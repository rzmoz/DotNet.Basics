namespace DotNet.Basics.Sys
{
    public class PathDir : PathInfo
    {
        public PathDir(string path, params string[] segments) : base(path, segments)
        {
        }

        public PathDir(string path, IsFolder isFolder, params string[] segments) : base(path, isFolder, segments)
        {
        }

        public PathDir(string path, IsFolder isFolder, PathSeparator pathSeparator, params string[] segments) : base(path, isFolder, pathSeparator, segments)
        {
        }
    }
}
