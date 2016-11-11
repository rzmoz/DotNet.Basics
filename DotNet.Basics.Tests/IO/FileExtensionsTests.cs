using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.TestSources;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class FileExtensionsTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testFile = @"\testc\file.txt";

        [Fact]
        public void ReadAllText_ReadTextFromFile_ContentIsRead()
        {
            var testdir = @"ReadAllTextAsync_ReadTextFromFile_ContentIsRead".ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt");

            string testContent = "Blaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah";

            testContent.WriteAllText(testFile);

            var read = testFile.ReadAllText();

            read.Should().Be(testContent);
        }

        [Fact]
        public void Delete_DeleteFile_FileIsDeleted()
        {
            var testdir = @"Delete_DeleteFile_FileIsDeleted".ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt");
            "blaa".WriteAllText(testFile);

            testFile.Exists().Should().BeTrue("File should have been created");

            testFile.DeleteIfExists();

            testFile.Exists().Should().BeFalse("File should have been deleted");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_EnsureTargetdir_TargetDirIsEnsured(bool ensureTargetDir)
        {
            var testdir = @"CopyTo_EnsureTargetdir_TargetDirIsEnsured".ToDir();
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
            var testdir = @"MoveTo_RenameFileInSameFolder_FileIsRenamed".ToDir();
            testdir.CleanIfExists();
            var sourceFile = testdir.ToFile("blaaOld.txt");
            var tagetFile = testdir.ToFile("blaaNew.txt");
            "blaa".WriteAllText(sourceFile);

            sourceFile.Exists().Should().BeTrue("Source file before move");
            tagetFile.Exists().Should().BeFalse("Target file before move");

            sourceFile.MoveTo(tagetFile);

            sourceFile.Exists().Should().BeFalse("Source file after move");
            tagetFile.Exists().Should().BeTrue("Target file after move");
        }

        [Fact]
        public void ToFile_CombineToFileInfo_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToFile(_testFile).FullName;
            const string expected = _testDirRoot + _testFile;
            actual.Should().Be(expected);
        }

        [Fact]
        public void ToFile_ParentFolderCombine_FileNameIsCombined()
        {
            var file = _testDoubleDir.ToFile(_testFile);
            file.FullName.Should().EndWith(_testDoubleDir + _testFile);
        }


        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullName.Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToFile(sourceFile.Name);

            targetfile.FullName.Should().Be(@"c:\MyPath\myFile.temp");
        }

        [Theory]
        [InlineData("MyFile", ".txt")]//has extension
        [InlineData("MyFile", "")]//no extension
        [InlineData("", ".txt")]//only extension
        [InlineData(null, ".txt")]//name is null
        [InlineData("MyFile", null)]//extension is null
        public void Extension_FileNameExtension_ExtensionIsRecognized(string name, string extension)
        {
            var expectedFullName = name + extension;

            var file = expectedFullName.ToFile();
            file.Name.Should().Be(expectedFullName, nameof(file.Name));
            file.NameWithoutExtension.Should().Be(name ?? "", "NameWithoutExtension");
            file.Extension.Should().Be(extension ?? "");
        }
    }
}
