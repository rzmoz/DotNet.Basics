using System;
using System.IO;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class SystemIoPathTests
    {
        [Fact]
        public void GetFullPath_LongPath_NoExceptionIsThrown()
        {
            var longPath = "GetFullPath_LongPath_NoExceptionIsThrownxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            var fullPath = SystemIoPath.GetFullPath(longPath);

            fullPath.Should().EndWith(longPath);//no exception is thrown
        }
        
        [Fact]
        public void GetFullPath_CallSystemIo_PathsAreIdentical()
        {
            var relativePath = "GetFullPath_CallSystemIo_PathsAreIdentical";

            var systemDotIoDotPath = System.IO.Path.GetFullPath(relativePath);
            var systemIoPath = SystemIoPath.GetFullPath(relativePath);

            systemIoPath.Should().Be(systemDotIoDotPath);
        }

        [Fact]
        public void Exists_ThrowIfNotFound_ExceptionIsThrown()
        {
            var path = @"Exists_ThrowIfNotFound_ExceptionIsThrown".ToDir();

            path.DeleteIfExists();

            Action action = () => SystemIoPath.Exists(path.FullName, path.IsFolder, throwIoExceptionIfNotExists: true);

            action.ShouldThrow<IOException>().WithMessage($"{path.FullName} not found");
        }

        [Fact]
        public void Exists_PathExists_Works()
        {
            var path = @"Exists_PathExists_Works".ToDir();

            path.DeleteIfExists();

            SystemIoPath.Exists(path.FullName, path.IsFolder).Should().BeFalse();

            path.CreateIfNotExists();

            SystemIoPath.Exists(path.FullName, path.IsFolder).Should().BeTrue();
        }

        [Fact]
        public void Exists_LongPaths_NoExceptionIsThrown()
        {
            var path = ".".ToDir("Exists_LongPaths_NoExceptionIsThrownxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            SystemIoPath.Exists(path.FullName, path.IsFolder).Should().BeFalse();
        }
    }
}
