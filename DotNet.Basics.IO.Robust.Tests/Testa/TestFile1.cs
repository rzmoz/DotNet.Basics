using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;

namespace DotNet.Basics.IO.Robust.Tests.Testa
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base(TestWithHelpers.TestRootDir, "Testa", "TextFile1.txt")
        {
        }
    }
}
