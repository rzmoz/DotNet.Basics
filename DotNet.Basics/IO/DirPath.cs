namespace DotNet.Basics.IO
{
    public class DirPath : Path
    {
        public DirPath(string fullPath)
            : this(null, new[] { fullPath }) { }
        public DirPath(string protocol, string[] pathTokens)
            : this(protocol, pathTokens, PathDelimiter.Backslash)
        { }
        public DirPath(string protocol, string[] pathTokens, PathDelimiter delimiter) : base(protocol, pathTokens, true, delimiter)
        {
        }
    }
}
