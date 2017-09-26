using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Sys
{
    public class PathFile : PathInfo
    {
        public PathFile(string path, params string[] segments) : base(path, segments)
        {
        }

        public PathFile(string path, char pathSeparator, params string[] segments) : base(path, pathSeparator, segments)
        {
        }
    }
}
