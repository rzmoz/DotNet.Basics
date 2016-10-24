using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class IoExtensionsTests
    {
        [Test]
        public void WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown()
        {
            var targetFile = TestContext.CurrentContext.TestDirectory.ToFile("WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown", "target.txt");
            targetFile.Directory.CreateIfNotExists();
            //ensure file exists
            File.WriteAllText(targetFile.FullName, @"mycontent");
            targetFile.Exists().Should().BeTrue();

            //act
            Action action = () => @"random".WriteAllText(targetFile, overwrite: false);

            action.ShouldThrow<IOException>().WithMessage($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullName}");
        }

        [Test]
        public void WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced()
        {
            var initialContent = "WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced";
            var updatedContent = "UpdatedContent";

            var targetFile = TestContext.CurrentContext.TestDirectory.ToFile(initialContent, "target.txt");
            targetFile.Directory.CreateIfNotExists();
            File.WriteAllText(targetFile.FullName, initialContent);
            targetFile.Exists().Should().BeTrue();
            File.ReadAllText(targetFile.FullName).Should().Be(initialContent);

            //act
            updatedContent.WriteAllText(targetFile, overwrite: true);

            File.ReadAllText(targetFile.FullName).Should().Be(updatedContent);
        }
    }
}
