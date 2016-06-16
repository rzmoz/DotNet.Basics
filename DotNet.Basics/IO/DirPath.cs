namespace DotNet.Basics.IO
{
    public class DirPath : Path
    {
        public DirPath(string fullPath)
            : this(new[] { fullPath })
        { }
        public DirPath(string[] pathSegments)
            : this(pathSegments, PathDelimiter.Backslash)
        { }
        public DirPath(string[] pathSegments, PathDelimiter delimiter)
            : base(pathSegments, true, delimiter)
        { }
    }
}
