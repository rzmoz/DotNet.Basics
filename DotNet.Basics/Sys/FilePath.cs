using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Sys
{
    public class FilePath : PathInfo
    {
        public FilePath(string path, params string[] segments) : base(path, segments)
        {
        }

        public FilePath(string path, IsFolder isFolder, params string[] segments) : base(path, isFolder, segments)
        {
        }

        public FilePath(string path, IsFolder isFolder, PathSeparator pathSeparator, params string[] segments) : base(path, isFolder, pathSeparator, segments)
        {
        }
    }
}
