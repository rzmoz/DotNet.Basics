using System;
using System.Collections.Generic;
using System.IO;
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
            NameWoExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
        }
        public string NameWoExtension { get; }
        public string Extension { get; }
    }
}
