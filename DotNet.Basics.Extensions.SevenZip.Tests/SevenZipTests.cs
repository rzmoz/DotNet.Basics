using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Extensions.SevenZip.Tests
{
    public class SevenZipTests : TestWithHelpers
    {
        private readonly SevenZipExe _sevenZip;

        public SevenZipTests(ITestOutputHelper output) : base(output)
        {
            _sevenZip = new SevenZipExe(TestRoot);
        }


        [Fact]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            //arrange
            var targetPath = TestRoot.ToFile("CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists", "myArchive.zip");
            targetPath.DeleteIfExists();
            targetPath.WriteAllText("dummyContent");
            targetPath.Exists().Should().BeTrue();

            //act
            Action action = () => _sevenZip.CreateFromDirectory("mySource", targetPath.FullName(), false);

            action.ShouldThrow<System.IO.IOException>();
        }

        [Fact]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            //arrange
            var sourceDir = TestRoot.ToDir("CreateFromDirectory_Zip_ContentIsZipped", "source");
            var dummyfile = sourceDir.ToFile("myfile.txt");
            var dummycontent = "dummyContent";

            dummyfile.WriteAllText(dummycontent);

            var targetZip = TestRoot.ToFile("CreateFromDirectory_Zip_ContentIsZipped", "myArchive.zip");
            targetZip.DeleteIfExists();

            targetZip.Exists().Should().BeFalse();

            //act
            _sevenZip.CreateFromDirectory(sourceDir.FullName(), targetZip.FullName());

            targetZip.Exists().Should().BeTrue($"Exists:{targetZip.FullName()}");
            using (var archive = ZipFile.OpenRead(targetZip.FullName()))
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
        public void ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown()
        {
            //arrange
            var archive = TestRoot.ToFile(@"ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "notexists.zip");
            var mockTargetDir = TestRoot.ToDir("ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "Mock");
            //act
            Action action = () => _sevenZip.ExtractToDirectory(archive.FullName(), mockTargetDir.FullName());
            action.ShouldThrow<IOException>().And.Message.Should().StartWith("Archive not found:");
        }

        [Fact]
        public void ExtractToDirectory_TargetDirAlreadyExists_ExceptionIsThrown()
        {
            //arrange
            var sourceZip = TestRoot.ToFile("myArchive.zip");
            var targetDir = TestRoot.ToDir("ExtractToDirectory_TargetDirAlreadyExists_ExceptionIsThrown", "Mock");
            targetDir.CreateIfNotExists();

            //act
            Action action = () => _sevenZip.ExtractToDirectory(sourceZip.FullName(), targetDir.FullName());
            action.ShouldThrow<IOException>().And.Message.Should().StartWith("Target dir already exists at:");
        }

        [Fact]
        public void ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir()
        {
            //arrange
            var targetDir = TestRoot.ToDir(@"ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir", "out");
            var targetFile = targetDir.ToFile("myfile.txt");
            var sourceZip = TestRoot.ToFile("myArchive.zip");
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse("arrange");
            targetFile.Exists().Should().BeFalse("arrange");

            //act

            var result = _sevenZip.ExtractToDirectory(sourceZip.FullName(), targetDir.FullName());
            Output.WriteLine($"ExtractToDirectory input: {result.Input}");
            Output.WriteLine($"ExtractToDirectory result: {result.Output}");
            targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");
            targetDir.ToDir().EnumeratePaths().Count().Should().Be(1);
            targetFile.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");

            var content = targetFile.ReadAllText();

            content.Should().Be("dummyContent");
        }
    }
}
