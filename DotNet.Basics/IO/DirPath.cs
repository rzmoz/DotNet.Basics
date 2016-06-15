namespace DotNet.Basics.IO
{
    public class DirPath : Path
    {
        public DirPath(string fullPath)
            : this(new[] { fullPath })
        { }
        public DirPath(string[] pathTokens)
            : this(pathTokens, PathDelimiter.Backslash)
        { }
        public DirPath(string[] pathTokens, PathDelimiter delimiter)
            : base(pathTokens, true, delimiter)
        { }
    }
}
