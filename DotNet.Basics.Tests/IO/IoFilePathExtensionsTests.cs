using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class IoFilePathExtensionsTests
    {
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned()
        {
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned").ToFile("NotFOund.asd");
            var content = file.ReadAllText(false);
            content.Should().BeNull();
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned()
        {
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned").ToFile("NotFOund.asd");
            file.Directory.CreateIfNotExists();
            var content = file.ReadAllText(false);
            content.Should().BeNull();
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenDirNotFound_ExceptionIsThrown()
        {
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_ThrowWhenDirNotFound_ExceptionIsThrown").ToFile("NotFOund.asd");
            Action action = () => file.ReadAllText();
            action.ShouldThrow<DirectoryNotFoundException>();
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenFileNotFound_ExceptionIsThrown()
        {
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_ThrowWhenFileNotFound_ExceptionIsThrown").ToFile("NotFOundxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.asd");
            file.Directory.CreateIfNotExists();
            Action action = () => file.ReadAllText();
            action.ShouldThrow<FileNotFoundException>();
        }

        [Fact]
        public void ReadAllText_ReadTextFromFile_ContentIsRead()
        {
            var testdir = TestRoot.Dir.Add(@"ReadAllTextAsync_ReadTextFromFile_ContentIsRead").ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt");

            string testContent = "Blaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah";

            testFile.WriteAllText(testContent);

            var read = testFile.ReadAllText();

            read.Should().Be(testContent);
        }

        [Fact]
        public void WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown()
        {
            var targetFile = TestRoot.Dir.ToFile(@"WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown", "target.txt");
            targetFile.Directory.CreateIfNotExists();
            //ensure file exists
            File.WriteAllText(targetFile.FullPath(), @"mycontent");
            targetFile.Exists().Should().BeTrue();

            //act
            Action action = () => targetFile.WriteAllText(@"random", overwrite: false);

            action.ShouldThrow<IOException>().WithMessage($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullPath()}");
        }

        [Fact]
        public void WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced()
        {
            var initialContent = "WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced";
            var updatedContent = "UpdatedContent";

            var targetFile = TestRoot.Dir.ToFile(@"WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced", "target.txt");
            targetFile.Directory.CreateIfNotExists();
            File.WriteAllText(targetFile.FullPath(), initialContent);
            targetFile.Exists().Should().BeTrue();
            File.ReadAllText(targetFile.FullPath()).Should().Be(initialContent);

            //act
            targetFile.WriteAllText(updatedContent, overwrite: true);

            File.ReadAllText(targetFile.FullPath()).Should().Be(updatedContent);
        }
    }
}
