using System.IO;
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
    }
}
