using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Sys
{
    public class PathDir : PathInfo
    {
        public PathDir(string path, params string[] segments) : base(path, segments)
        {
        }

        public PathDir(string path, char pathSeparator, params string[] segments) : base(path, pathSeparator, segments)
        {
        }
    }
}
