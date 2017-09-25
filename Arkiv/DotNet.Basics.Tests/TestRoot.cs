using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;

namespace DotNet.Basics.Tests
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = typeof(TestRoot).GetTypeInfo().Assembly.Location;
            var dir = Path.GetDirectoryName(entryPath);
            CurrentDir = dir.ToPath();
        }

        public static PathInfo CurrentDir { get; }
    }
}
