using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.IO.Testa
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base(TestRoot.CurrentDir.RawPath, "IO", "Testa", "TextFile1.txt")
        {
        }
    }
}
