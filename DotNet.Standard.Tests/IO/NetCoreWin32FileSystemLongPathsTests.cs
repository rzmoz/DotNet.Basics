using System.IO;
using System.Linq;
using DotNet.Standard.IO;
using DotNet.Standard.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Standard.Tests.IO
{
    public class NetCoreWin32FileSystemLongPathsTests : LongPathsFileSystemTests
    {
        public NetCoreWin32FileSystemLongPathsTests(ITestOutputHelper output)
            : base(new NetCoreWin32FileSystemLongPaths(), output)
        { }

        //paths
        [Fact]
        public void Enumerates_Paths_PatshAreFound()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var output = $"Current path length is {testDir.FullName().Length} : {testDir.FullName()} ";
                Output.WriteLine(output);

                testDir.CreateSubDir("1");
                testDir.CreateSubDir("2");
                testDir.CreateSubDir("3");
                testDir.ToFile("myFile1.txt").WriteAllText("bla");
                testDir.ToFile("myFile2.txt").WriteAllText("bla");

                var paths = FileSystem.EnumeratePaths(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
                var dirs = FileSystem.EnumerateDirectories(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
                var files = FileSystem.EnumerateFiles(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();

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
