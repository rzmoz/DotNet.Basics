using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class NetCoreWin32FileSystemLongPathTests : FileSystemTests
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\";

        public NetCoreWin32FileSystemLongPathTests(ITestOutputHelper output)
            : base(new NetCoreWin32FileSystemLongPaths(), output, _veryLongPath)
        {
        }

        [Fact]
        public void Enumerates_Paths_PatshAreFound()
        {
            ArrangeActAssertPaths(testDir =>
            {
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
