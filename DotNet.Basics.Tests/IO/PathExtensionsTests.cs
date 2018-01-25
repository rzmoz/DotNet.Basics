using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class PathExtensionsTests : TestWithHelpers
    {
        public PathExtensionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void ToPath_DirToDir_PathTypeIsSet()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var isDir = dir.ToDir("folder");

                //pre-assert
                isDir.PathType.Should().Be(PathType.Dir);

                //act
                var pi = isDir.ToPath(PathType.Dir);

                //assert
                pi.PathType.Should().Be(PathType.Dir);

                isDir.RawPath.Should().Be(pi.RawPath);
            });
        }

        [Fact]
        public void ToPath_DirToFile_PathTypeIsSet()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var isDir = dir.ToDir("folder");

                //pre-assert
                isDir.PathType.Should().Be(PathType.Dir);

                //act
                var pi = isDir.ToPath(PathType.File);

                //assert
                pi.PathType.Should().Be(PathType.File);

                isDir.RawPath.Should().StartWith(pi.RawPath);
            });
        }

        [Fact]
        public void ToPath_FileToDir_PathTypeIsSet()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var isFile= dir.ToFile("file.txt");

                //pre-assert
                isFile.PathType.Should().Be(PathType.File);

                //act
                var pi = isFile.ToPath(PathType.Dir);

                //assert
                pi.PathType.Should().Be(PathType.Dir);

                pi.RawPath.Should().StartWith(isFile.RawPath);
            });
        }

        [Fact]
        public void ToPath_FileToFile_PathTypeIsSet()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var isFile = dir.ToFile("file.txt");

                //pre-assert
                isFile.PathType.Should().Be(PathType.File);

                //act
                var pi = isFile.ToPath(PathType.File);

                //assert
                pi.PathType.Should().Be(PathType.File);

                pi.RawPath.Should().Be(isFile.RawPath);
            });
        }

        [Fact]
        public void ToPath_DetectDirWhenExists_TypeIsResolved()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var pathName = "DirWithoutTrailingSeparator";
                pathName.ToDir().DeleteIfExists();
                //pre-assert
                var rawPi = pathName.ToPath();
                pathName.ToDir().CreateIfNotExists();

                //act
                var pi = pathName.ToPath();

                //assert
                rawPi.PathType.Should().Be(PathType.File);
                pi.PathType.Should().Be(PathType.Dir);
            });
        }

        [Fact]
        public void ToPath_DetectFileWhenExists_TypeIsResolved()
        {
            ArrangeActAssertPaths(dir =>
            {
                //arrange
                var pathName = "FileWithTrailingSeparator.txt/";
                pathName.ToFile().DeleteIfExists();
                //pre-assert
                var rawPi = pathName.ToPath();
                pathName.ToFile().WriteAllText("bla");

                //act
                var pi = pathName.ToPath();

                //assert
                rawPi.PathType.Should().Be(PathType.Dir);
                pi.PathType.Should().Be(PathType.File);
            });
        }
    }
}
