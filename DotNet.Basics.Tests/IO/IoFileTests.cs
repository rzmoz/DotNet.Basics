using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class IoFileTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testFile = @"\testc\file.txt";

        [Test]
        public async Task ReadAllTextAsync_ReadTextFromFile_ContentIsRead()
        {
            var testdir = "ReadAllTextAsync_ReadTextFromFile_ContentIsRead".ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt");

            string testContent = "Blaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah";

            testContent.WriteToDisk(testFile);

            var read = await testFile.ReadAllTextAsync().ConfigureAwait(false);

            read.Should().Be(testContent);
        }

        [Test]
        public void Delete_DeleteFile_FileIsDeleted()
        {
            var testdir = "Delete_DeleteFile_FileIsDeleted".ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt");
            "blaa".WriteToDisk(testFile);

            testFile.Exists().Should().BeTrue();
            
            testFile.DeleteIfExists();

            testFile.Exists().Should().BeFalse();
        }

        [Test]
        public void MoveTo_RenameFileInSameFolder_FileIsRenamed()
        {
            var testdir = "MoveTo_RenameFileInSameFolder_FileIsRenamed".ToDir();
            testdir.CleanIfExists();
            var sourceFile = testdir.ToFile("blaaOld.txt");
            var tagetFile = testdir.ToFile("blaaNew.txt");
            "blaa".WriteToDisk(sourceFile);

            sourceFile.Exists().Should().BeTrue();
            tagetFile.Exists().Should().BeFalse();

            sourceFile.MoveTo(tagetFile);

            sourceFile.Exists().Should().BeFalse();
            tagetFile.Exists().Should().BeTrue();
        }

        [Test]
        public void ToFile_CombineToFileInfo_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToFile(_testFile).FullName;
            const string expected = _testDirRoot + _testFile;
            actual.Should().Be(expected);
        }

        [Test]
        public void ToFile_ParentFolderCombine_FileNameIsCombined()
        {
            var file = _testDoubleDir.ToFile(_testFile);
            file.FullName.Should().EndWith(_testDoubleDir + _testFile);
        }


        [Test]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile);

            targetfile.FullName.Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Test]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToDir().ToFile(sourceFile);

            targetfile.FullName.Should().Be(@"c:\MyPath\myFile.temp");
        }


        [Test]
        public void Extension_HasExtension_ExtensionIsFound()
        {
            const string fileName = "MyFile";
            const string extension = ".txt";

            var file = new IoFile(fileName + extension);
            file.Name.Should().Be(fileName + extension);
            file.NameWithoutExtension.Should().Be(fileName);
            file.Extension.Should().Be(extension);
        }
        [Test]
        public void Extension_NoExtension_ExtensionIsEmpty()
        {
            const string fileName = "MyFile";
            var file = new IoFile(fileName);
            file.Name.Should().Be(fileName);
            file.NameWithoutExtension.Should().Be(fileName);
            file.Extension.Should().BeEmpty();
        }
        [Test]
        public void Extension_OnlyExtension_ExtensionIsEmpty()
        {
            const string extension = ".txt";
            var file = new IoFile(extension);
            file.Name.Should().Be(extension);
            file.NameWithoutExtension.Should().BeEmpty();
            file.Extension.Should().Be(extension);
        }
    }
}
