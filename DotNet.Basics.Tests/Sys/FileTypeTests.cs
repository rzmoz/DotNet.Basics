using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FileTypeTests
    {
        [Fact]
        public void Ctor_ExtensionWithDot_ExtensionHasDot()
        {
            const string extensionWithDot = ".xml";

            var fileType = new FileType("xml", extensionWithDot);

            fileType.Extension.Should().Be(extensionWithDot);

        }
        [Fact]
        public void Ctor_ExtensionWithoutDot_ExtensionHasDot()
        {
            const string extensionWithoutDot = "xml";

            var fileType = new FileType("xml", extensionWithoutDot);

            fileType.Extension.Should().Be("." + extensionWithoutDot);
        }
        
    }
}
