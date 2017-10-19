using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class FilePathTests
    {
        private const string _path = "c:/mypath";
        private const string _segment = "segment";

        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testFile = @"\testc\file.txt";

        [Fact]
        public void Add_File_SameTypeIsReturned()
        {
            var dir = _path.ToFile().Add(_segment);

            dir.Should().BeOfType<FilePath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }");
        }
        [Fact]
        public void ToFile_Create_FileIsCreated()
        {
            var file = _path.ToFile().ToDir().ToFile(_segment);//different extension methods

            file.Should().BeOfType<FilePath>();
            file.RawPath.Should().Be(_path + $"/{_segment }");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_EnsureTargetdir_TargetDirIsEnsured(bool ensureTargetDir)
        {
            var testdir = TestRoot.Dir.Add(@"CopyTo_EnsureTargetdir_TargetDirIsEnsured");
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

        [Fact]
        public void MoveTo_RenameFileInSameFolder_FileIsRenamed()
        {
            var testdir = TestRoot.Dir.Add(@"MoveTo_RenameFileInSameFolder_FileIsRenamed").ToDir();
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
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned").ToFile("NotFOund.asd");
            var content = file.ReadAllText(false);
            content.Should().BeNull();
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned()
        {
            var file = TestRoot.Dir.Add(@"ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned").ToFile("NotFOund.asd");
            file.Directory().CreateIfNotExists();
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
            file.Directory().CreateIfNotExists();
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
            targetFile.Directory().CreateIfNotExists();
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
            targetFile.Directory().CreateIfNotExists();
            File.WriteAllText(targetFile.FullPath(), initialContent);
            targetFile.Exists().Should().BeTrue();
            File.ReadAllText(targetFile.FullPath()).Should().Be(initialContent);

            //act
            targetFile.WriteAllText(updatedContent, overwrite: true);

            File.ReadAllText(targetFile.FullPath()).Should().Be(updatedContent);
        }


        [Theory]
        [InlineData("SomeDir\\MyFile.txt", "MyFile")]//has extension
        [InlineData("SomeDir\\MyFile", "MyFile")]//no extension
        [InlineData("SomeDir\\.txt", "")]//only extension
        [InlineData(null, "")]//name is null
        public void NameWoExtension_WithoutExtension_NameIsRight(string name, string nameWoExtensions)
        {
            var file = name.ToFile();
            file.NameWoExtension.Should().Be(nameWoExtensions);
        }
        [Theory]
        [InlineData("SomeDir\\MyFile.txt", ".txt")]//has extension
        [InlineData("SomeDir\\MyFile", "")]//no extension
        [InlineData("SomeDir\\.txt", ".txt")]//only extension
        [InlineData(null, "")]//name is null
        public void Extension_Extension_ExtensionsIsRight(string name, string extension)
        {
            var file = name.ToFile();
            file.Extension.Should().Be(extension);
        }

        [Fact]
        public void ToFile_CombineToFileInfo_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToFile(_testFile).FullPath();
            const string expected = _testDirRoot + _testFile;
            actual.Should().Be(expected);
        }

        [Fact]
        public void ToFile_ParentFolderCombine_FileNameIsCombined()
        {
            var file = _testDoubleDir.ToFile(_testFile);
            file.FullPath().Should().EndWith(_testDoubleDir + _testFile);
        }
        
        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullPath().Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToFile(sourceFile.Name);

            targetfile.FullPath().Should().Be(@"c:\MyPath\myFile.temp");
        }
    }
}
