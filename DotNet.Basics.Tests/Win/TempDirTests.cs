using DotNet.Basics.Sys;
using DotNet.Basics.Win;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.IO;
using Xunit.Abstractions;
using Xunit;

namespace DotNet.Basics.Tests.Win
{
    public class TempDirTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        [Fact]
        public void Ctor_RandomNess_RandomDirsAreGenerated()
        {
            ArrangeActAssertPaths(testDir =>
            {
                const int numOfDirsToGenerate = 23;//prime
                var rootDir = testDir.ToDir("MAH");
                var dirs = new Dictionary<string, TempDir>();//dic to ensure names are unique
                foreach (var i in Enumerable.Range(1, numOfDirsToGenerate))
                {
                    var td = new TempDir(rootDir);
                    dirs.Add(td.Root.Name, td);
                }

                dirs.Count.Should().Be(numOfDirsToGenerate);
                foreach (var td in dirs)
                {
                    using var tempDir = td.Value;
                    tempDir.Root.Exists().Should().BeTrue(tempDir.Root.FullName);
                }
            });
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
