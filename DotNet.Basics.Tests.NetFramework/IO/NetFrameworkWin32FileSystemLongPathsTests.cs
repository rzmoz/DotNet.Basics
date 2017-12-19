using DotNet.Basics.IO;
using DotNet.Basics.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.NetFramework.IO
{
    public class NetFrameworkWin32FileSystemLongPathsTests : FileSystemTests
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\";

        public NetFrameworkWin32FileSystemLongPathsTests(ITestOutputHelper output)
            : base(new NetFrameworkWin32FileSystemLongPaths(), output, _veryLongPath)
        {
        }
    }
}
