using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class TempDirTests
    {/*
        [Fact]
        public void Ctor_RandomNess_RandomDirsAreGenerated()
        {
            const int numOfDirsToGenerate = 23;//prime
            var rootDir = @"Ctor_RandomNess_RandomDirsAreGenerated".ToDir();
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
                    tempDir.Root.Exists().Should().BeTrue(tempDir.Root.FullName);
                }
            }
        }
    }*/
}
