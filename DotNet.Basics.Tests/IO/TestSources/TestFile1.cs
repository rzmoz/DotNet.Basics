using DotNet.Basics.IO;

namespace DotNet.Basics.Tests.IO.TestSources
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base("IO".ToFile("TestSources", "TextFile1.txt").FullName)
        {
        }
    }
}
