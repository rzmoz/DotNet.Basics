using DotNet.Basics.IO;
using DotNet.Basics.Tests.NetFramework;

namespace DotNet.Basics.Tests.IO.Testa
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base(TestRoot.Dir, "IO", "NET47_Testa", "TextFile1.txt")
        {
        }
    }
}
