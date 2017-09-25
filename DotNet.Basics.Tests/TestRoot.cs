using System.IO;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.Win32;
using DotNet.Basics.Tests.Sys;

namespace DotNet.Basics.Tests
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = typeof(ExecutableTests).GetTypeInfo().Assembly.Location;
            var dir = Path.GetDirectoryName(entryPath);
            CurrentDir = dir.ToPath();
        }

        public static PathInfo CurrentDir { get; }
    }
}
