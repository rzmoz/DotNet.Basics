using DotNet.Basics.IO;
using DotNet.Basics.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class NetCoreWin32FileSystemLongPathTests : FileSystemTests
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\";

        public NetCoreWin32FileSystemLongPathTests(ITestOutputHelper output)
            : base(new NetCoreWin32FileSystemLongPaths(), output, _veryLongPath)
        {
        }
    }
}
