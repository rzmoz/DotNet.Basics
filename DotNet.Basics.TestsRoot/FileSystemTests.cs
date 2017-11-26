using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.TestsRoot
{
    public abstract class FileSystemTests : TestWithHelpers
    {
        private readonly IFileSystem _fileSystem;

        protected FileSystemTests(IFileSystem fileSystem, ITestOutputHelper output, string testPathPrefix = null) : base(output, testPathPrefix)
        {
            _fileSystem = fileSystem;
        }

        //paths
        [Fact]
        public void GetFullPath_Paths_FullPathIsResolved()
        {
            WithTestRoot(testRoot =>
            {
                var path = TestPathPrefix + "MyPath";

                //act
                var result = _fileSystem.GetFullPath(path);
                //assert
                result.Should().Be(new DirectoryInfo(path).FullName);
            });
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

                var paths = _fileSystem.EnumeratePaths(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
                var dirs = _fileSystem.EnumerateDirectories(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();
                var files = _fileSystem.EnumerateFiles(testDir.FullName(), "*", SearchOption.AllDirectories).ToList();

                paths.Count.Should().Be(5);
                paths.Count.Should().Be(testDir.GetPaths().Count);

                dirs.Count.Should().Be(3);
                dirs.Count.Should().Be(testDir.GetDirectories().Count);

                files.Count.Should().Be(2);
                files.Count.Should().Be(testDir.GetFiles().Count);
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
                _fileSystem.CreateDir(testDir.FullName());
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
                sourceDir.CreateSubDir("hello");
                sourceDir.ToFile("MyFile.txt").WriteAllText("ewewer");
                sourceDir.EnumeratePaths().Count().Should().Be(2);
                sourceDir.Exists().Should().BeTrue();

                var targetDir = testDir.ToDir("Target");
                targetDir.DeleteIfExists();

                //act
                _fileSystem.MoveDir(sourceDir.FullName(), targetDir.FullName());
                //assert
                sourceDir.Exists().Should().BeFalse();
                targetDir.EnumeratePaths().Count().Should().Be(2);
            });
        }

        [Fact]
        public void Exists_Dirs_NoExceptionIsThrown()
        {
            var path = TestPathPrefix + "path";
            //act
            var result = _fileSystem.ExistsFile(path) || _fileSystem.ExistsDir(path);
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
                _fileSystem.DeleteDir(testDir.FullName());
                //assert
                testDir.Exists().Should().BeFalse();
            });
        }

        //files
        [Fact]
        public void CopyFile_Dirs_FileIsCopied()
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

                _fileSystem.CopyFile(sourceFile.FullName(), targetFile.FullName(), true);

                sourceFile.Exists().Should().BeTrue();
                targetFile.Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void MoveFile_Dirs_FileIsMoved()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var sourceFile = testDir.ToFile("Source", "MySource.txt");
                var targetFile = testDir.ToFile("Target", "MyTarget.txt");
                sourceFile.WriteAllText("blaaa");
                targetFile.DeleteIfExists();
                targetFile.Exists().Should().BeFalse();
                targetFile.Directory().CreateIfNotExists();
                _fileSystem.MoveFile(sourceFile.FullName(), targetFile.FullName());

                sourceFile.Exists().Should().BeFalse();
                targetFile.Exists().Should().BeTrue();
            });
        }

        [Fact]
        public void ExistsFile_Dirs_FileExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("myfile.dat");
                file.DeleteIfExists();

                _fileSystem.ExistsFile(file.FullName()).Should().BeFalse();

                file.WriteAllText("aassd");
                _fileSystem.ExistsFile(file.FullName()).Should().BeTrue();
            });
        }

        [Fact]
        public void DeleteFile_Dirs_FileExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("myfile.dat");
                file.WriteAllText("aassd");
                file.Exists().Should().BeTrue();

                _fileSystem.DeleteFile(file.FullName());

                file.Exists().Should().BeFalse();
            });
        }
    }
}
