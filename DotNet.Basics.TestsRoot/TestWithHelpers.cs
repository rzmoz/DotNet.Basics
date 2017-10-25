using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace DotNet.Basics.TestsRoot
{
    public class TestWithHelpers
    {
        static TestWithHelpers()
        {
            var entryPath = typeof(TestWithHelpers).GetTypeInfo().Assembly.Location;
            TestRootDir = Path.GetDirectoryName(entryPath);
        }

        protected TestWithHelpers(ITestOutputHelper output)
        {
            Output = output;

        }

        public ITestOutputHelper Output { get; }
        public string TestRoot => TestRootDir;

        public static string TestRootDir { get; }
    }
}
