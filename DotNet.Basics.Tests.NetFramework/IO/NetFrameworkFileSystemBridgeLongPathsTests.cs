using DotNet.Basics.IO;
using DotNet.Basics.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.NetFramework.IO
{
    public class NetFrameworkFileSystemBridgeLongPathsTests : FileSystemTests
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum";

        public NetFrameworkFileSystemBridgeLongPathsTests(ITestOutputHelper output)
            : base(output, _veryLongPath)
        {
        }
    }
}
