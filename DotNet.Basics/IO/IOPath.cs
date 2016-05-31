using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class IOPath : Path
    {
        public IOPath(string path) : base(path)
        {
            Delimiter = PathDelimiter.Backslash;
        }
        
        public IOPath(string path, bool isFolder) : base(path, isFolder)
        {
            Delimiter = PathDelimiter.Backslash;
        }
    }
}
