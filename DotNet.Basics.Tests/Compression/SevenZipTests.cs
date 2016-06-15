using System;
using System.IO;
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
        [Test]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            //arrange
            var targetPath = "CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists".ToFilePath("myArchive.zip");
            targetPath.DeleteIfExists();
            "dummyContent".WriteAllText(targetPath);
            targetPath.Exists().Should().BeTrue();
            var zip = new SevenZip();
            //act
            Action action = () => zip.CreateFromDirectory("mySource", targetPath.FullName, false);

            action.ShouldThrow<IOException>();
        }

        [Test]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            //arrange
            var sourceDir = "CreateFromDirectory_Zip_ContentIsZipped".ToDirPath("source");
            var dummyfile = sourceDir.Add("myfile.txt").ToFilePath();
            var dummycontent = "dummyContent";

            dummycontent.WriteAllText(dummyfile);

            var targetZip = "CreateFromDirectory_Zip_ContentIsZipped".ToFilePath("myArchive.zip");
            targetZip.DeleteIfExists();

            targetZip.Exists().Should().BeFalse();
            var zip = new SevenZip();
            //act
            zip.CreateFromDirectory(sourceDir.FullName, targetZip.FullName);

            targetZip.Exists().Should().BeTrue($"Exists:{targetZip.FullName}");
            using (var archive = ZipFile.OpenRead(targetZip.FullName))
            {
                archive.Entries.Count.Should().Be(1);

                using (var stream = archive.Entries.Single().Open())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
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
            var targetDir = "ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir".ToDirPath("out");
            var targetFile = targetDir.Add("myfile.txt").ToFilePath();
            var sourceZip = "compression".ToFilePath("myArchive.zip");
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();
            targetFile.Exists().Should().BeFalse();

            var zip = new SevenZip();
            //act
            zip.ExtractToDirectory(sourceZip.FullName, targetDir.FullName);
            targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullName}");
            targetDir.ToDir().EnumerateFileSystemInfos().Count().Should().Be(1);
            targetFile.Exists().Should().BeTrue();

            var content = File.ReadAllText(targetFile.FullName);

            content.Should().Be("dummyContent");
        }
    }
}
