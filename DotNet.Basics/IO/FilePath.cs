using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class FilePath : Path
    {
        public FilePath(string fullPath)
            : this(new[] { fullPath })
        { }
        public FilePath(string[] pathTokens)
            : this(pathTokens, PathDelimiter.Backslash)
        { }
        public FilePath(string[] pathTokens, PathDelimiter delimiter)
            : base(pathTokens, false, delimiter)
        { }
    }
}
