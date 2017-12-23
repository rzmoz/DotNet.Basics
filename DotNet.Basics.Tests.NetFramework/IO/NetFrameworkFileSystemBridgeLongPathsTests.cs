using DotNet.Basics.IO;
using DotNet.Basics.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.NetFramework.IO
{
    public class NetFrameworkFileSystemBridgeLongPathsTests : LongPathsFileSystemTests
    {
        public NetFrameworkFileSystemBridgeLongPathsTests(ITestOutputHelper output)
            : base(new NetFrameworkWin32FileSystemLongPaths(), output)
        {
        }
    }
}
