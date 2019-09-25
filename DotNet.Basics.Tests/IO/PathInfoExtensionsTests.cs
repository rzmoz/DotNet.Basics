using System;
using System.IO;
using DotNet.Basics.Tests.IO.Testa;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class PathInfoExtensionsTests : TestWithHelpers
    {
        public PathInfoExtensionsTests(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [InlineData(@"inetpub\dir", false)] // relative path
        [InlineData(@"c:\inetpub", true)] //absolute path
        [InlineData(@"\\myServer\some\dir", true)]//UNC path
        public void IsRooted_Detection_DetectsIfPathIsRooted(string path, bool expected)
        {
            var asPath = path.ToPath();

            asPath.IsRooted().Should().Be(expected, path);
        }

        [Fact]
        public void DeleteIfExists_DirExists_DirIsDeleted()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var dir = testDir.Add("maah");

                dir.CreateIfNotExists();
                dir.Exists().Should().BeTrue();

                //act
                dir.DeleteIfExists();

                //assert
                dir.Exists().Should().BeFalse();
            });
        }

        [Fact]
        public void DeleteIfExists_DeleteLongNamedDir_DirIsDeleted()
        {
            ArrangeActAssertPaths(testDir =>
            {

                //arrange
                testDir.CleanIfExists();
                var identicalSubDir = testDir;
                try
                {
                    //we keep adding sub folders until we reach our limit - whichever comes first
                    for (var level = 0; level < 15; level++)
                    {
                        identicalSubDir.CreateIfNotExists();
                        identicalSubDir = identicalSubDir.ToDir(testDir.Name);
                    }
                }
                catch (System.IO.PathTooLongException)
                {
                    identicalSubDir.CleanIfExists();
                }

                testDir.Exists().Should().BeTrue();

                //act
                var deleted = testDir.DeleteIfExists();

                //assert
                deleted.Should().BeTrue();
                testDir.Exists().Should().BeFalse();
            });
        }

        [Fact]
        public void Delete_DeleteFile_FileIsDeleted()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var dir = testDir.Add("Mah");
                dir.CleanIfExists();
                var testFile = dir.ToFile("blaaaah.txt").WriteAllText("Blaaaah!");

                testFile.Exists().Should().BeTrue("File should have been created");

                testFile.DeleteIfExists();

                testFile.Exists().Should().BeFalse("File should have been deleted");
            });
        }

        [Fact]
        public void GetFullPath_AssertWithSystemIo_PathsAreIdentical()
        {
            var relativePath = "GetFullPath_CallSystemIo_PathsAreIdentical";

            var systemDotIoDotPath = Path.GetFullPath(relativePath);
            var systemIoPath = relativePath.ToPath().FullName();

            systemIoPath.Should().Be(systemDotIoDotPath);
        }

        [Fact]
        public void Exists_ThrowIfNotFound_ExceptionIsNotThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var path = testDir.ToDir("MAH");

                path.DeleteIfExists();

                Action action = () => path.Exists();

                action.Should().NotThrow();
            });
        }

        [Fact]
        public void Exists_PathExists_Works()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var path = testDir.Add("MAH");
                path.DeleteIfExists();

                path.Exists().Should().BeFalse();

                path.CreateIfNotExists();

                path.Exists().Should().BeTrue();
            });

        }

        [Fact]
        public void Exists_LongDirPath_NoExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var path = testDir.ToDir("DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\");
                path.Exists().Should().BeFalse();
            });
        }

        [Fact]
        public void Exists_LongFilePath_NoExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var path = testDir.ToFile("DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm.txt");
                path.Exists().Should().BeFalse();
            });
        }

        [Theory]
        //relative dir
        [InlineData(".\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //absolute dir
        [InlineData("c:\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //relative file
        [InlineData(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        //absolute file
        [InlineData("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        public void FullPath_LongPath_LongPathsAreSupported(string p)
        {
            Action action = () => p.ToPath().FullName();

            var temp = p.ToPath().FullName();//no exceptions are thrown

            action.Should().NotThrow();
        }

        [Fact]
        public void FullPath_Resolve_FullPathIsResolved()
        {
            TestFile1 inputPath = null;
            WithTestRoot(testRoot => inputPath = new TestFile1(testRoot));
            string expectedPath = null;
            WithTestRoot(testRoot => expectedPath = new TestFile1(testRoot).FullName());

            inputPath.Should().NotBe(expectedPath);

            inputPath.FullName().ToLowerInvariant().Should().Be(expectedPath.ToLowerInvariant());
        }
    }
}

