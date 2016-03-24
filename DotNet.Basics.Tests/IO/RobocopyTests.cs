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

            var testFile = "blaa".WriteToDisk(dir, "robocopyTestfile.text");

            var exitCode = Robocopy.MoveFile(testFile, testFile.Directory);

            exitCode.Should().Be(0);
        }

        [Test]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = "IO\\TestSources".ToDir().ToFile("TextFile1.txt");
            sourcefile.Exists().Should().BeTrue("source file should exist");


            var targetFile = "IO\\TestTargets".ToDir().ToFile($"{sourcefile.Name}");
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");
            Robocopy.Copy(sourcefile, targetFile, FileCopyOptions.OverwriteIfExists);
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

    }
}
