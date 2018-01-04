using DotNet.Standard.IO;
using DotNet.Standard.TestsRoot;
using Xunit.Abstractions;

namespace DotNet.Standard.Tests.NetFramework.IO
{
    public class NetFrameworkFileSystemBridgeLongPathsTests : LongPathsFileSystemTests
    {
        public NetFrameworkFileSystemBridgeLongPathsTests(ITestOutputHelper output)
            : base(new NetFrameworkWin32FileSystemLongPaths(), output)
        { }
    }
}
