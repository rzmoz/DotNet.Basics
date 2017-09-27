using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
public class IoDirPathExtensionsTests
    {
        /*
        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = @"CreateIfExists_CreateOptions_ExistingDirIsCleaned".ToDir();
            @"bllll".WriteAllText(testDir.ToFile("myFile.txt"));

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists();
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(0);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = @"CreateIfExists_CreateOptions_ExistingDirIsNotCleaned".ToDir();
            testDir.DeleteIfExists();
            @"bllll".WriteAllText(testDir.ToFile("myFile.txt"));

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);
        }*/
    }
}
