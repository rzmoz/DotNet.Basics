using System.IO;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.IO.Robust.Tests
{
    public class NetCoreLongPathTests : TestWithHelpers
    {
        public NetCoreLongPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void EnumeratePaths_Enumerate_PatshAreFound()
        {
            var testDir = TestRoot.ToDir("EnumeratePaths_Enumerate_PatshAreFound");
            testDir.CreateSubDir("1");
            testDir.CreateSubDir("2");
            testDir.CreateSubDir("3");
            testDir.ToFile("myFile1.txt").WriteAllText("bla");
            testDir.ToFile("myFile2.txt").WriteAllText("bla");

            var paths = NetCoreLongPath.EnumeratePaths(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
            var dirs = NetCoreLongPath.EnumerateDirectories(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
            var files = NetCoreLongPath.EnumerateFiles(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();

            paths.Count.Should().Be(5);
            paths.Count.Should().Be(testDir.GetPaths().Count);

            dirs.Count.Should().Be(3);
            dirs.Count.Should().Be(testDir.GetDirectories().Count);

            files.Count.Should().Be(2);
            files.Count.Should().Be(testDir.GetFiles().Count);
        }

        [Fact]
        public void CreateDirectory_VeryLongPaths_DirIsCreated()
        {
            var veryLongPath = TestRoot.ToDir("CreateDirectory_VeryLongPaths_NoExceptionIsThrown", @"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd");
            veryLongPath.DeleteIfExists();
            veryLongPath.Exists().Should().BeFalse();
            //act
            NetCoreLongPath.CreateDir(veryLongPath.FullName());
            //assert
            veryLongPath.Exists().Should().BeTrue();
        }

        [Fact]
        public void Exists_VeryLongPaths_NoExceptionIsThrown()
        {
            var veryLongPath = @"c:\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd";
            //act
            var result = NetCoreLongPath.Exists(veryLongPath);
            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetfullName_VeryLongPaths_FullNameIsResolved()
        {
            var veryLongPath = @"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd";
            //act
            var result = NetCoreLongPath.NormalizePath(veryLongPath);
            //assert
            result.Should().Be($@"{TestRoot}\{veryLongPath}");
        }

        [Fact]
        public void TryDelete_DeleteDir_DirIsDeleted()
        {
            var veryLongPath = TestRoot.ToDir(@"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd");
            veryLongPath.CreateIfNotExists();
            veryLongPath.Exists().Should().BeTrue();
            //act
            NetCoreLongPath.DeleteDir(veryLongPath.FullName());
            //assert
            veryLongPath.Exists().Should().BeFalse();
        }


    }
}
