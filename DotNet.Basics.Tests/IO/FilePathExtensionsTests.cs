using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.Testa;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class FilePathExtensionsTests : TestWithHelpers
    {
        public FilePathExtensionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_EnsureTargetdir_TargetDirIsEnsured(bool ensureTargetDir)
        {
            var testdir = TestRoot.ToDir($@"CopyTo_EnsureTargetdir_TargetDirIsEnsured_{ensureTargetDir}");
            testdir.DeleteIfExists();
            var testFile1 = new TestFile1();
            Action action = () => testFile1.CopyTo(testdir, false, ensureTargetDir);

            if (ensureTargetDir)
            {
                action.ShouldNotThrow();
                testdir.ToFile(testFile1.Name).Exists().Should().BeTrue();
            }
            else
            {
                action.ShouldThrow<IOException>();
                testdir.ToFile(testFile1.Name).Exists().Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_OverWrite_OverwriteIsObeyed(bool overwrite)
        {
            var testDir = TestRoot.ToDir($@"CopyTo_OverWrite_OverwriteIsObeyed_{overwrite}");
            var targetFile = testDir.ToFile("myFile.txt");
            targetFile.WriteAllText("blaaah!");
            targetFile.ReadAllText().Should().Be("blaaah!");//assert target file exists

            var testFile1 = new TestFile1();
            testFile1.CopyTo(testDir, targetFile.Name, true);//ensure file exists in target

            Action action = () => testFile1.CopyTo(targetFile, overwrite);

            if (overwrite)
            {
                action.ShouldNotThrow();
                targetFile.ReadAllText().Should().Be("Hello World!");
            }
            else
            {
                action.ShouldThrow<IOException>();
            }
        }



        [Fact]
        public void MoveTo_RenameFileInSameFolder_FileIsRenamed()
        {
            var testdir = TestRoot.ToDir(@"MoveTo_RenameFileInSameFolder_FileIsRenamed");
            testdir.CleanIfExists();
            var sourceFile = testdir.ToFile("blaaOld.txt");
            var tagetFile = testdir.ToFile("blaaNew.txt");
            sourceFile.WriteAllText("blaa");

            sourceFile.Exists().Should().BeTrue("Source file before move");
            tagetFile.Exists().Should().BeFalse("Target file before move");

            sourceFile.MoveTo(tagetFile);

            sourceFile.Exists().Should().BeFalse("Source file after move");
            tagetFile.Exists().Should().BeTrue("Target file after move");
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned()
        {
            var file = TestRoot.ToFile(@"ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned", "NotFOund.asd");
            var content = file.ReadAllText(IfNotExists.Mute);
            content.Should().BeNull();
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned()
        {
            var file = TestRoot.ToFile(@"ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned", "NotFOund.asd");
            file.Directory().CreateIfNotExists();
            var content = file.ReadAllText(IfNotExists.Mute);
            content.Should().BeNull();
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenDirNotFound_ExceptionIsThrown()
        {
            var file = TestRoot.ToFile(@"ReadAllTextThrowIfNotExists_ThrowWhenDirNotFound_ExceptionIsThrown").ToFile("NotFOund.asd");
            Action action = () => file.ReadAllText();
            action.ShouldThrow<DirectoryNotFoundException>();
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenFileNotFound_ExceptionIsThrown()
        {
            var file = TestRoot.ToFile(@"ReadAllTextThrowIfNotExists_ThrowWhenFileNotFound_ExceptionIsThrown").ToFile("NotFOundxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.asd");
            file.Directory().CreateIfNotExists();
            Action action = () => file.ReadAllText();
            action.ShouldThrow<FileNotFoundException>();
        }

        [Fact]
        public void ReadAllText_ReadTextFromFile_ContentIsRead()
        {
            var testdir = TestRoot.ToDir(@"ReadAllTextAsync_ReadTextFromFile_ContentIsRead");
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
            var targetFile = TestRoot.ToFile(@"WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown", "target.txt");
            targetFile.Directory().CreateIfNotExists();
            //ensure file exists
            File.WriteAllText(targetFile.FullName(), @"mycontent");
            targetFile.Exists().Should().BeTrue();

            //act
            Action action = () => targetFile.WriteAllText(@"random", overwrite: false);

            action.ShouldThrow<IOException>().WithMessage($"Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullName()}");
        }

        [Fact]
        public void WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced()
        {
            var initialContent = "WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced";
            var updatedContent = "UpdatedContent";

            var targetFile = TestRoot.ToFile(@"WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced", "target.txt");
            targetFile.Directory().CreateIfNotExists();
            File.WriteAllText(targetFile.FullName(), initialContent);
            targetFile.Exists().Should().BeTrue();
            File.ReadAllText(targetFile.FullName()).Should().Be(initialContent);

            //act
            targetFile.WriteAllText(updatedContent, overwrite: true);

            File.ReadAllText(targetFile.FullName()).Should().Be(updatedContent);
        }

        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullName().Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToFile(sourceFile.Name);

            targetfile.FullName().Should().Be(@"c:\MyPath\myFile.temp");
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
