using System.Linq;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using FluentAssertions;
using NLog.Targets;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class RobocopyTests
    {
        [Test]
        public void Init_FindRobocopyByDrive_RobocopyIsFound()
        {
            var dir = TestContext.CurrentContext.TestDirectory.ToDir(@"Init_FindRobocopyByDrive_RobocopyIsFound");
            dir.CleanIfExists();

            var testFile = "blaa".WriteAllText(dir, "robocopyTestfile.text");

            var exitCode = Robocopy.MoveContent(testFile.Directory.FullName, testFile.Directory.FullName, testFile.Name);

            exitCode.Should().Be(0);
        }

        [Test]
        public void MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved()
        {
            var emptyDir = TestContext.CurrentContext.TestDirectory.ToDir("MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved", "empty");
            var sourceDir = TestContext.CurrentContext.TestDirectory.ToDir("MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved", "source");
            var targetDir = TestContext.CurrentContext.TestDirectory.ToDir("MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved", "target");
            var testSource = TestContext.CurrentContext.TestDirectory.ToDir("IO", "TestSources").FullName;
            Robocopy.CopyDir(testSource, sourceDir.FullName, true, null);
            emptyDir.CreateIfNotExists();
            emptyDir.CleanIfExists();
            emptyDir.GetPaths().Length.Should().Be(0);//empty dir
            sourceDir.Exists().Should().BeTrue(sourceDir.FullName);
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse(targetDir.FullName);

            //act
            Robocopy.MoveContent(sourceDir.FullName, targetDir.FullName, null, true, null);
            Robocopy.MoveContent(emptyDir.FullName, targetDir.FullName, null, true, null);//move empty dir  to ensure target dir is not cleaned

            //assert
            sourceDir.GetPaths().Count().Should().Be(0);
            targetDir.GetFiles().Single().Name.Should().Be("TextFile1.txt");
        }

        [Test]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = TestContext.CurrentContext.TestDirectory.ToFile("IO\\TestSources", "TextFile1.txt");
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = TestContext.CurrentContext.TestDirectory.ToFile("Copy_CopySingleFileSourceExists_FileIsCopied", sourcefile.Name);
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");
            var result = Robocopy.CopyFile(sourcefile.FullName, targetFile.Directory.FullName);
            result.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

        [Test]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            var source = TestContext.CurrentContext.TestDirectory.ToDir("CopyDir_CopyDirSourceExists_DirIsCopied", "source");
            var target = TestContext.CurrentContext.TestDirectory.ToDir("CopyDir_CopyDirSourceExists_DirIsCopied", "target");
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
