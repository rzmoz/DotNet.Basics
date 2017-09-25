namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return new PathInfo(path, segments);
        }
    }
}
