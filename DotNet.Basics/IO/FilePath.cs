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
        public FilePath(string[] pathSegments)
            : this(pathSegments, PathDelimiter.Backslash)
        { }
        public FilePath(string[] pathSegments, PathDelimiter delimiter)
            : base(pathSegments, false, delimiter)
        { }
    }
}
