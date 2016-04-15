using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class FileTypeTests
    {
        [Test]
        public void Ctor_ExtensionWithDot_ExtensionHasDot()
        {
            const string extensionWithDot = ".xml";

            var fileType = new FileType("xml", extensionWithDot);

            fileType.Extension.Should().Be(extensionWithDot);

        }
        [Test]
        public void Ctor_ExtensionWithoutDot_ExtensionHasDot()
        {
            const string extensionWithoutDot = "xml";

            var fileType = new FileType("xml", extensionWithoutDot);

            fileType.Extension.Should().Be("." + extensionWithoutDot);
        }

        [Test]
        public void IsType_TwoPartExtensionIsSupport_FileNameIsDetected()
        {
            const string twoPartExtension = ".config.disabled";
            const string fileName = "myFile" + twoPartExtension;

            var fileType = new FileType("TwoPartExtensionFilyType", twoPartExtension);

            fileName.ToFile().IsFileType(fileType).Should().BeTrue();
        }
    }
}
