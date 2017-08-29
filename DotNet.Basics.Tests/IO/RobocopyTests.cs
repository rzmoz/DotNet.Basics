using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.TestSources;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class RobocopyTests
    {
        [Fact]
        public void Init_FindRobocopyByDrive_RobocopyIsFound()
        {

            var dir = TestRoot.CurrentDir.Add(@"Init_FindRobocopyByDrive_RobocopyIsFound").ToDir();
            var testFile = dir.ToFile("robocopyTestfile.text");
            dir.CleanIfExists();

            "blaa".WriteAllText(testFile);

            var exitCode = Robocopy.MoveContent(testFile.Directory.FullName, testFile.Directory.FullName, testFile.Name);

            exitCode.Should().Be(0);
        }

        [Fact]
        public void MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved()
        {
            var baseDir = TestRoot.CurrentDir.Add(@"MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved");
            var emptyDir = baseDir.ToDir("empty");
            var sourceDir = baseDir.ToDir("source");
            var targetDir = baseDir.ToDir("target");
            var testSource = new TestFile1();
            Robocopy.CopyDir(testSource.Directory.FullName, sourceDir.FullName, true, null);
            emptyDir.CreateIfNotExists();
            emptyDir.CleanIfExists();
            emptyDir.GetPaths().Length.Should().Be(0);//empty dir
            sourceDir.Exists().Should().BeTrue(sourceDir.FullName);
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse(targetDir.FullName);

            //act
            Robocopy.MoveContent(sourceDir.FullName, targetDir.FullName, null, true, null);
            Robocopy.MoveContent(emptyDir.FullName, targetDir.FullName, null, true, null);//move empty dir to ensure target dir is not cleaned

            //assert
            sourceDir.Exists().Should().BeTrue(sourceDir.FullName);
            sourceDir.IsEmpty();
            targetDir.GetFiles().Single().Name.Should().Be(testSource.Name);
        }

        [Fact]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = new TestFile1();
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = TestRoot.CurrentDir.Add(@"Copy_CopySingleFileSourceExists_FileIsCopied").ToFile(sourcefile.Name);
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");
            var result = Robocopy.CopyFile(sourcefile.FullName, targetFile.Directory.FullName);
            result.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

        [Fact]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            var source = TestRoot.CurrentDir.Add(@"CopyDir_CopyDirSourceExists_DirIsCopied").ToDir("source");
            var target = TestRoot.CurrentDir.Add(@"CopyDir_CopyDirSourceExists_DirIsCopied").ToDir("target");
            var sourceFile = source.ToFile("myfile.txt");
            var targetFile = source.ToFile(sourceFile.Name);
            var fileContent = "blavlsavlsdglsdflslfsdlfsdlfsd";
            target.DeleteIfExists();
            target.Exists().Should().BeFalse();

            source.CreateIfNotExists();
            fileContent.WriteAllText(sourceFile);
            sourceFile.Exists().Should().BeTrue();

            //act
            var result = Robocopy.CopyDir(source.FullName, target.FullName, true);

            //assert
            result.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            target.Exists().Should().BeTrue();
            targetFile.Exists().Should().BeTrue();
            targetFile.ReadAllText().Should().Be(fileContent);
        }
    }
}
