using System.IO;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.Shell;

namespace DotNet.Basics.Tests
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = typeof(ExecutableTests).GetTypeInfo().Assembly.Location;
            var dir = Path.GetDirectoryName(entryPath);
            Dir = dir.ToDir();
        }

        public static DirPath Dir { get; }
    }
}
