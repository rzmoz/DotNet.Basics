
using DotNet.Basics.IO;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    public class TestFile1 : FilePath
    {
        public TestFile1() : base(TestContext.CurrentContext.TestDirectory.ToFile("IO", "TestSources", "TextFile1.txt").FullName)
        {
        }
    }
}
