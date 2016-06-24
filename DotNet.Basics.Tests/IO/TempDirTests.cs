using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;


namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class TempDirTests
    {
        [Test]
        public void Ctor_RandomNess_RandomDirsAreGenerated()
        {
            const int numOfDirsToGenerate = 23;//prime
            var rootDir = TestContext.CurrentContext.TestDirectory.ToDir("Ctor_RandomNess_RandomDirsAreGenerated");
            var dirs = new List<TempDir>();
            Parallel.For(0, numOfDirsToGenerate, i =>
            {
                dirs.Add(new TempDir(rootDir));
            });

            dirs.Count.Should().Be(numOfDirsToGenerate);
            foreach (var tempDir in dirs)
            {
                tempDir.Root.Exists().Should().BeTrue(tempDir.Root.FullName);
            }

            foreach (var tempDir in dirs)
            {
                tempDir.Dispose();
            }
        }
    }
}
