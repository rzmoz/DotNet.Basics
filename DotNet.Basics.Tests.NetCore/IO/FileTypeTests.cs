using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
public    class FileTypeTests
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

        [Fact]
        public void IsType_TwoPartExtensionIsSupport_FileNameIsDetected()
        {
            const string twoPartExtension = ".config.disabled";
            const string fileName = "myFile" + twoPartExtension;

            var fileType = new FileType("TwoPartExtensionFilyType", twoPartExtension);

            fileName.ToFile().IsFileType(fileType).Should().BeTrue();
        }
    }
}
