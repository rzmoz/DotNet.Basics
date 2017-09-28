using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class IoDirPathExtensionsTests
    {
        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("CreateIfExists_CreateOptions_ExistingDirIsCleaned");
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);

            //act
            testDir.CreateIfNotExists();
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(0);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("CreateIfExists_CreateOptions_ExistingDirIsNotCleaned");
            testDir.DeleteIfExists();
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);

            //act
            testDir.CreateIfNotExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);
        }
    }
}
