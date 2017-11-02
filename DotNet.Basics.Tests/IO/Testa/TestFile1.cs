using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.IO.Testa
{
    public class TestFile1 : FilePath
    {
        public TestFile1(DirPath testRootPath) : base(@"IO\Testa", "TextFile1.txt")
        {
        }
    }
}
