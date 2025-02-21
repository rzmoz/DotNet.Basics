using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class NetCoreWin32FileSystemLongPathsTests(ITestOutputHelper output) : LongPathsFileSystemTests(output)
    {
        //paths
        [Fact]
        public void Enumerates_Paths_PathsAreFound()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var output = $"Current path length is {testDir.FullName.Length} : {testDir.FullName} ";
                Output.WriteLine(output);

                testDir.CreateSubDir("1");
                testDir.CreateSubDir("2");
                testDir.CreateSubDir("3");
                testDir.ToFile("myFile1.txt").WriteAllText("bla");
                testDir.ToFile("myFile2.txt").WriteAllText("bla");

                var paths = Directory.EnumerateFileSystemEntries(testDir.FullName, "*", SearchOption.AllDirectories).ToList();
                var dirs = Directory.EnumerateDirectories(testDir.FullName, "*", SearchOption.AllDirectories).ToList();
                var files = Directory.EnumerateFiles(testDir.FullName, "*", SearchOption.AllDirectories).ToList();

                paths.Count.Should().Be(5);
                paths.Count.Should().Be(testDir.GetPaths().Count);

                dirs.Count.Should().Be(3);
                dirs.Count.Should().Be(testDir.GetDirectories().Count);

                files.Count.Should().Be(2);
                files.Count.Should().Be(testDir.GetFiles().Count);
            });
        }
    }
}
