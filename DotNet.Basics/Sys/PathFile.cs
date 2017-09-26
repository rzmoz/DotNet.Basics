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

        public PathFile(string path, IsFolder isFolder, params string[] segments) : base(path, isFolder, segments)
        {
        }

        public PathFile(string path, IsFolder isFolder, PathSeparator pathSeparator, params string[] segments) : base(path, isFolder, pathSeparator, segments)
        {
        }
    }
}
