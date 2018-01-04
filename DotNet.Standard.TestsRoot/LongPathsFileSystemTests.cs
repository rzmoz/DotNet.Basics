using System.IO;
using DotNet.Standard.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Standard.TestsRoot
{
    public abstract class LongPathsFileSystemTests : TestWithHelpers
    {
        private const string _veryLongPath = @"loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\loremp\ipsum\";
        protected readonly IFileSystem FileSystem;

        protected LongPathsFileSystemTests(IFileSystem fileSystem, ITestOutputHelper output) : base(output, _veryLongPath)
        {
            FileSystem = fileSystem;
        }
        
        [Fact]
        public void GetFullPath_Paths_FullPathIsResolved()
        {
            WithTestRoot(testRoot =>
            {
                var path = TestPathPrefix + "MyPath";

                //act
                var result = FileSystem.GetFullPath(path);
                //assert
                result.Should().Be(Path.Combine(new DirectoryInfo(".").FullName, path));
            });
        }

        //dirs
        [Fact]
        public void CreateDirectory_Dirs_DirIsCreated()
        {
            ArrangeActAssertPaths(testDir =>
            {
                testDir.DeleteIfExists();
                testDir.Exists().Should().BeFalse();
                //act
                FileSystem.CreateDir(testDir.FullName());
                //assert
                testDir.Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void MoveDir_Dirs_DirIsMoved()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var sourceDir = testDir.ToDir("Source");
                sourceDir.CleanIfExists();
                var helloDir = sourceDir.CreateSubDir("hello");
                var myFile = sourceDir.ToFile("MyFile.txt").WriteAllText("ewewer");

                sourceDir.Exists().Should().BeTrue();
                helloDir.Exists().Should().BeTrue();
                myFile.Exists().Should().BeTrue();

                var targetDir = testDir.ToDir("Target");
                targetDir.DeleteIfExists();

                //act
                FileSystem.MoveDir(sourceDir.FullName(), targetDir.FullName());
                //assert
                sourceDir.Exists().Should().BeFalse();
                targetDir.Add(helloDir.Name).Exists().Should().BeTrue();
                targetDir.ToFile(myFile.Name).Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void Exists_Dirs_NoExceptionIsThrown()
        {
            var path = TestPathPrefix + "path";
            //act
            var result = FileSystem.ExistsFile(path) || FileSystem.ExistsDir(path);
            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void DeleteDir_Dirs_DirIsDeleted()
        {
            ArrangeActAssertPaths(testDir =>
            {
                testDir.CreateIfNotExists();
                testDir.Exists().Should().BeTrue();
                //act
                FileSystem.DeleteDir(testDir.FullName());
                //assert
                testDir.Exists().Should().BeFalse();
            });
        }

        //files
        [Fact]
        public void CopyFile_Files_FileIsCopied()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var sourceFile = testDir.ToFile("Source", "MySource.txt");
                var targetFile = testDir.ToFile("Target", "MyTarget.txt");
                sourceFile.Directory().CreateIfNotExists();
                targetFile.DeleteIfExists();
                targetFile.Directory().CreateIfNotExists();
                sourceFile.WriteAllText("blaaa");
                targetFile.Exists().Should().BeFalse();

                FileSystem.CopyFile(sourceFile.FullName(), targetFile.FullName(), true);

                sourceFile.Exists().Should().BeTrue();
                targetFile.Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void MoveFile_Files_FileIsMoved()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var sourceFile = testDir.ToFile("Source", "MySource.txt");
                var targetFile = testDir.ToFile("Target", "MyTarget.txt");
                sourceFile.WriteAllText("blaaa");
                targetFile.DeleteIfExists();
                targetFile.Exists().Should().BeFalse();
                targetFile.Directory().CreateIfNotExists();
                FileSystem.MoveFile(sourceFile.FullName(), targetFile.FullName());

                sourceFile.Exists().Should().BeFalse();
                targetFile.Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void ExistsFile_Files_FileExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var testFile = testDir.ToFile("tempfile.dat");
                testFile.WriteAllText("blaaa");
                testFile.Exists().Should().BeTrue();

                testFile.Exists().Should().BeTrue();
                //act
                FileSystem.DeleteFile(testFile.FullName());

                //assert
                testFile.Exists().Should().BeFalse();
            });
        }

        [Fact]
        public void DeleteFile_Files_FileExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var testFile = testDir.ToFile("tempfile.dat");
                testFile.WriteAllText("blaaa");
                testFile.Exists().Should().BeTrue();

                //act
                FileSystem.DeleteFile(testFile.FullName());

                //assert
                testFile.Exists().Should().BeFalse();
            });
        }
    }
}
