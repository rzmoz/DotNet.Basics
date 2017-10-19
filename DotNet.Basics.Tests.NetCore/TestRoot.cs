using System.IO;
using System.Reflection;
using DotNet.Basics.IO;

namespace DotNet.Basics.Tests
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = typeof(TestRoot).GetTypeInfo().Assembly.Location;
            var dir = Path.GetDirectoryName(entryPath);
            Dir = dir.ToDir();
        }

        public static DirPath Dir { get; }
    }
}
