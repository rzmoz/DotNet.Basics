using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class RobocopyTests
    {
        [Test]
        public void Init_FindRobocopyByDrive_RobocopyIsFound()
        {
            var dir = @"Init_FindRobocopyByDrive_RobocopyIsFound".ToDir();
            dir.CleanIfExists();

            var testFile = "blaa".WriteAllText(dir, "robocopyTestfile.text");

            var exitCode = Robocopy.Move(testFile.Directory.FullName, testFile.Directory.FullName, testFile.Name);

            exitCode.Should().Be(0);
        }

        [Test]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = "IO\\TestSources".ToFile("TextFile1.txt");
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = "IO\\TestTargets".ToFile($"{sourcefile.Name}");
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");
            var result = Robocopy.CopyFile(sourcefile.FullName, targetFile.Directory.FullName);
            result.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }
        [Test]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            var source = "CopyDir_CopyDirSourceExists_DirIsCopied".ToDir("source");
            var target = "CopyDir_CopyDirSourceExists_DirIsCopied".ToDir("target");
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
