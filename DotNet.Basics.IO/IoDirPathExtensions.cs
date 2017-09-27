using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoDirPathExtensions
    {
        public static void CreateIfNotExists(this DirPath dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (dir.Exists())
                return;

            System.IO.Directory.CreateDirectory(dir.FullPath());
        }
    }
}
