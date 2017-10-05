using System;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNet.Basics.Extensions.SevenZip;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Extensions.SevenZip
{
    public class SevenZipTests
    {
        private readonly SevenZipExe _sevenZip;

        public SevenZipTests()
        {
            _sevenZip = new SevenZipExe(TestRoot.Dir);
        }

        [Fact]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            //arrange
            var targetPath = TestRoot.Dir.ToFile("CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists", "myArchive.zip");
            targetPath.DeleteIfExists();
            targetPath.WriteAllText("dummyContent");
            targetPath.Exists().Should().BeTrue();

            //act
            Action action = () => _sevenZip.CreateFromDirectory("mySource", targetPath.FullPath(), false);

            action.ShouldThrow<System.IO.IOException>();
        }

        [Fact]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            //arrange
            var sourceDir = TestRoot.Dir.Add("CreateFromDirectory_Zip_ContentIsZipped", "source");
            var dummyfile = sourceDir.ToFile("myfile.txt");
            var dummycontent = "dummyContent";

            dummyfile.WriteAllText(dummycontent);

            var targetZip = TestRoot.Dir.ToFile("CreateFromDirectory_Zip_ContentIsZipped", "myArchive.zip");
            targetZip.DeleteIfExists();

            targetZip.Exists().Should().BeFalse();

            //act
            _sevenZip.CreateFromDirectory(sourceDir.FullPath(), targetZip.FullPath());

            targetZip.Exists().Should().BeTrue($"Exists:{targetZip.FullPath()}");
            using (var archive = ZipFile.OpenRead(targetZip.FullPath()))
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

        [Fact]
        public void ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir()
        {
            //arrange
            var targetDir = TestRoot.Dir.Add(@"ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir", "out");
            var targetFile = targetDir.ToFile("myfile.txt");
            var sourceZip = TestRoot.Dir.ToFile(@"Extensions", "SevenZip", "myArchive.zip");
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();
            targetFile.Exists().Should().BeFalse();

            //act
            _sevenZip.ExtractToDirectory(sourceZip.FullPath(), targetDir.FullPath());
            targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullPath()}");
            targetDir.ToDir().EnumeratePaths().Count().Should().Be(1);
            targetFile.Exists().Should().BeTrue();

            var content = targetFile.ReadAllText();

            content.Should().Be("dummyContent");
        }
    }
}
