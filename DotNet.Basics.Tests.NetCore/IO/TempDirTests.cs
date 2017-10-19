using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class TempDirTests
    {
        [Fact]
        public void Ctor_RandomNess_RandomDirsAreGenerated()
        {
            const int numOfDirsToGenerate = 23;//prime
            var rootDir = TestRoot.Dir.Add("Ctor_RandomNess_RandomDirsAreGenerated");
            var dirs = new Dictionary<string, TempDir>();//dic to ensure names are unique
            foreach (var i in Enumerable.Range(1, numOfDirsToGenerate))
            {
                var td = new TempDir(rootDir);
                dirs.Add(td.Root.Name, td);
            }

            dirs.Count.Should().Be(numOfDirsToGenerate);
            foreach (var td in dirs)
            {
                using (var tempDir = td.Value)
                {
                    tempDir.Root.Exists().Should().BeTrue(tempDir.Root.FullPath());
                }
            }
        }
        [Fact]
        public void Use_Dir_DirExists()
        {
            PathInfo dir = null;

            using (var temp = new TempDir("TempDirTest"))
            {
                dir = temp.Root;
                dir.Exists().Should().BeTrue();
            }

            dir.Exists().Should().BeFalse();
        }
    }
}
