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
            : this(null, new[] { fullPath }) { }
        public FilePath(string protocol, string[] pathTokens)
            : this(protocol, pathTokens, PathDelimiter.Backslash)
        { }
        public FilePath(string protocol, string[] pathTokens, PathDelimiter delimiter)
            : base(protocol, pathTokens, false, delimiter)
        {
        }
    }
}
