using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class PathExtensionsTests : TestWithHelpers
    {
        public PathExtensionsTests(ITestOutputHelper output) : base(output)
        {
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
                pi.PathType.Should().Be(PathType.Folder);
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
                rawPi.PathType.Should().Be(PathType.Folder);
                pi.PathType.Should().Be(PathType.File);
            });
        }
    }
}
