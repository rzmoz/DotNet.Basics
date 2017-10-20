using System.IO;
using System.Reflection;

namespace DotNet.Basics.Tests.NetFramework
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = Directory.GetCurrentDirectory();
            Dir = Path.GetDirectoryName(entryPath);
        }

        public static string Dir { get; }
    }
}
