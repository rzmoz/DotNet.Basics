using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class SystemIoPathTests
    {
        [Test]
        public void GetFullPath_LongPath_NoExceptionIsThrown()
        {
            var longPath= "GetFullPath_LongPath_NoExceptionIsThrownxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            
            var fullPath = SystemIoPath.GetFullPath(longPath);
            
            fullPath.Should().EndWith(longPath);//no exception is thrown
        }


        [Test]
        public void GetFullPath_CallSystemIo_PathsAreIdentical()
        {
            var relativePath = "GetFullPath_CallSystemIo_PathsAreIdentical";

            var systemDotIoDotPath = System.IO.Path.GetFullPath(relativePath);
            var systemIoPath = SystemIoPath.GetFullPath(relativePath);

            systemIoPath.Should().Be(systemDotIoDotPath);
        }

        [Test]
        public void Exists_PathExists_Works()
        {
            var path = TestContext.CurrentContext.TestDirectory.ToDir("Exists_PathExists_Works");

            path.DeleteIfExists();

            SystemIoPath.Exists(path).Should().BeFalse();

            path.CreateIfNotExists();

            SystemIoPath.Exists(path).Should().BeTrue();
        }

        [Test]
        public void Exists_LongPaths_NoExceptionIsThrown()
        {
            var path = TestContext.CurrentContext.TestDirectory.ToDir("Exists_LongPaths_NoExceptionIsThrownxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            
            SystemIoPath.Exists(path).Should().BeFalse();
        }
    }
}
