using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Sys
{
    public class FilePath : PathInfo
    {
        public FilePath(string path, params string[] segments) 
            : this(path, PathSeparator.Unknown, segments)
        {
        }

        public FilePath(string path, PathSeparator pathSeparator, params string[] segments)
            : base(path, Sys.IsFolder.False, pathSeparator, segments)
        {
        }
    }
}
