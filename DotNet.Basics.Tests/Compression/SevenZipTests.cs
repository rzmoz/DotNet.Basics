using System;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNet.Basics.Compression;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Compression
{
    [TestFixture]
    public class SevenZipTests
    {
        private SevenZip _sevenZip;

        [SetUp]
        public void SetUp()
        {
            _sevenZip = new SevenZip(TestContext.CurrentContext.TestDirectory.ToDir());
        }

        [Test]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            //arrange
            var targetPath = TestContext.CurrentContext.TestDirectory.ToDir("CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists").ToFile("myArchive.zip");
            targetPath.DeleteIfExists();
            "dummyContent".WriteAllText(targetPath);
            targetPath.Exists().Should().BeTrue();

            //act
            Action action = () => _sevenZip.CreateFromDirectory("mySource", targetPath.FullName, false);

            action.ShouldThrow<System.IO.IOException>();
        }

        [Test]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            //arrange
            var sourceDir = TestContext.CurrentContext.TestDirectory.ToDir("CreateFromDirectory_Zip_ContentIsZipped", "source");
            var dummyfile = sourceDir.Add("myfile.txt").ToFile();
            var dummycontent = "dummyContent";

            dummycontent.WriteAllText(dummyfile);

            var targetZip = TestContext.CurrentContext.TestDirectory.ToDir("CreateFromDirectory_Zip_ContentIsZipped").ToFile("myArchive.zip");
            targetZip.DeleteIfExists();

            targetZip.Exists().Should().BeFalse();

            //act
            _sevenZip.CreateFromDirectory(sourceDir.FullName, targetZip.FullName);

            targetZip.Exists().Should().BeTrue($"Exists:{targetZip.FullName}");
            using (var archive = ZipFile.OpenRead(targetZip.FullName))
            {
                archive.Entries.Count.Should().Be(1);

                using (var stream = archive.Entries.Single().Open())
                using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    var read = reader.ReadToEnd();
                    read.Should().Be(dummycontent);
                }
            }
        }

        [Test]
        public void ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir()
        {
            //arrange
            var targetDir = TestContext.CurrentContext.TestDirectory.ToDir("ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir", "out");
            var targetFile = targetDir.Add("myfile.txt").ToFile();
            var sourceZip = TestContext.CurrentContext.TestDirectory.ToDir("compression").ToFile("myArchive.zip");
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();
            targetFile.Exists().Should().BeFalse();
            
            //act
            _sevenZip.ExtractToDirectory(sourceZip.FullName, targetDir.FullName);
            targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullName}");
            targetDir.ToDir().EnumeratePaths().Count().Should().Be(1);
            targetFile.Exists().Should().BeTrue();

            var content = targetFile.ReadAllText();

            content.Should().Be("dummyContent");
        }
    }
}
