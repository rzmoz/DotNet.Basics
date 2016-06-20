using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class TempWorkDirTests
    {
        [Test]
        public void Use_Dir_DirExists()
        {
            Path dir = null;

            using (var temp = new TempDir("TempDirTest"))
            {
                dir = temp.Root;
                dir.Exists().Should().BeTrue();
            }

            dir.Exists().Should().BeFalse();
        }
    }
}
