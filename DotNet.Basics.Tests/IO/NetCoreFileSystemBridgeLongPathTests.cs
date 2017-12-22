using DotNet.Basics.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class NetCoreFileSystemBridgeLongPathTests : FileSystemTests
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\";

        public NetCoreFileSystemBridgeLongPathTests(ITestOutputHelper output)
            : base(output, _veryLongPath)
        {
        }
    }
}
