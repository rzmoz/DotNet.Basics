using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class IoPathInfoExtensionsTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testSingleDir = @"\testk\";

        [Fact]
        public void DeleteIfExists_DirExists_DirIsDeleted()
        {
            var dir = TestRoot.Dir.ToDir("DeleteIfExists_DirExists_DirIsDeleted");
            dir.CreateIfNotExists();
            dir.Exists().Should().BeTrue();

            //act
            dir.DeleteIfExists();

            //assert
            dir.Exists().Should().BeFalse();
        }


        [Fact]
        public void DeleteIfExists_DeleteLongNamedDir_DirIsDeleted()
        {
            //arrange
            //we set up a folder with a really long name
            const string testDirName = "DeleteIfExists_DeleteLongNamedDir_DirIsDeleted";

            var rootTestdir = TestRoot.Dir.ToDir(testDirName);
            rootTestdir.CleanIfExists();
            var identicalSubDir = rootTestdir;
            try
            {
                //we keep adding sub folders until we reach our limit - whichever comes first
                for (var level = 0; level < 15; level++)
                {
                    identicalSubDir.CreateIfNotExists();
                    identicalSubDir = identicalSubDir.ToDir(testDirName);
                }
            }
            catch (System.IO.PathTooLongException)
            {
                identicalSubDir.CleanIfExists();
            }

            rootTestdir.Exists().Should().BeTrue();

            //act
            var deleted = rootTestdir.DeleteIfExists();

            //assert
            deleted.Should().BeTrue();
            rootTestdir.Exists().Should().BeFalse();
        }

        [Fact]
        public void Delete_DeleteFile_FileIsDeleted()
        {
            var testdir = TestRoot.Dir.Add(@"Delete_DeleteFile_FileIsDeleted").ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt").WriteAllText("Blaaaah!");

            testFile.Exists().Should().BeTrue("File should have been created");

            testFile.DeleteIfExists();

            testFile.Exists().Should().BeFalse("File should have been deleted");
        }


        [Fact]
        public void GetFullPath_CallSystemIo_PathsAreIdentical()
        {
            var relativePath = "GetFullPath_CallSystemIo_PathsAreIdentical";

            var systemDotIoDotPath = System.IO.Path.GetFullPath(relativePath);
            var systemIoPath = IoPathInfoExtensions.GetFullPath(relativePath);

            systemIoPath.Should().Be(systemDotIoDotPath);
        }

        [Fact]
        public void Exists_ThrowIfNotFound_ExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToDir("Exists_ThrowIfNotFound_ExceptionIsThrown");

            path.DeleteIfExists();

            Action action = () => path.Exists(throwIoExceptionIfNotExists: true);

            action.ShouldThrow<IOException>().WithMessage($"{path.FullPath()} not found");
        }

        [Fact]
        public void Exists_PathExists_Works()
        {
            var path = TestRoot.Dir.ToDir("Exists_PathExists_Works");

            path.DeleteIfExists();

            path.Exists().Should().BeFalse();

            path.CreateIfNotExists();

            path.Exists().Should().BeTrue();
        }

        [Fact]
        public void Exists_LongDirPath_NoExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToDir("DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\");
            path.Exists().Should().BeFalse();
        }
        [Fact]
        public void Exists_LongFilePath_NoExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToFile("FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");
            path.Exists().Should().BeFalse();
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
            var path = p.ToPath();

            var expectedP = IoPathInfoExtensions.GetFullPath(p);

            path.FullPath().Should().Be(expectedP);//no exceptions
        }

        [Fact]
        public void FullPath_Resolve_FullPathIsResolved()
        {
            var inputPath = TestRoot.Dir.ToFile("io","testa", "TextFile1.txt");
            var expectedPath = new TestFile1().RawPath;
            inputPath.Should().NotBe(expectedPath);

            inputPath.FullPath().ToLowerInvariant().Should().Be(expectedPath.ToLowerInvariant());
        }
    }
}
