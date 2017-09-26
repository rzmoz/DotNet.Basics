using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoPathDirExtensions
    {
        public static void CreateIfNotExists(this PathDir dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (dir.Exists())
                return;
            
        }
    }
}
