using System.IO;
using System.Reflection;

namespace DotNet.Basics.Tests.Net
{
    public static class TestRoot
    {
        static TestRoot()
        {
            var entryPath = typeof(TestRoot).GetTypeInfo().Assembly.Location;
            Dir = Path.GetDirectoryName(entryPath);
        }

        public static string Dir { get; }
    }
}
