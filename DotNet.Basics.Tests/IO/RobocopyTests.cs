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
            var sourcefile = "IO\\TestSources".ToDir().ToFile("TextFile1.txt");
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = "IO\\TestTargets".ToDir().ToFile($"{sourcefile.Name}");
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");
            var result = Robocopy.CopyFile(sourcefile.Directory.FullName, targetFile.Directory.FullName,sourcefile.Name);
            result.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

    }
}
