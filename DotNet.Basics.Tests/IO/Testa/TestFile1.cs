using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;

namespace DotNet.Basics.Tests.IO.Testa
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base(TestWithHelpers.TestRootDir, @"IO\Testa", "TextFile1.txt")
        {
        }
    }
}
