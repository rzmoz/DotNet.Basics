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

namespace DotNet.Basics.SevenZip.Sdk.Tests
{
    public class SevenZipTests : TestWithHelpers
    {
        private SevenZipExe _sevenZip;
        private FilePath _sourceArchive;

        public SevenZipTests(ITestOutputHelper output) : base(output)
        {
            WithTestRoot(testRoot =>
            {
                _sevenZip = new SevenZipExe(testRoot);
                _sourceArchive = testRoot.ToFile("myArchive.zip");
            });
        }

        [Fact]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var targetPath = testDir.ToFile("myArchive.zip");
                targetPath.DeleteIfExists();
                targetPath.WriteAllText("dummyContent");
                targetPath.Exists().Should().BeTrue();

                //act
                Action action = () => _sevenZip.CreateFromDirectory("mySource", targetPath.FullName(), false);

                action.ShouldThrow<System.IO.IOException>();
            });

        }

        [Fact]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var sourceDir = testDir.ToDir("CreateFromDirectory_Zip_ContentIsZipped", "source");
                var dummyfile = sourceDir.ToFile("myfile.txt");
                var dummycontent = "dummyContent";

                dummyfile.WriteAllText(dummycontent);

                var targetZip = testDir.ToFile("CreateFromDirectory_Zip_ContentIsZipped", "myArchive.zip");
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
            });
        }

        [Fact]
        public void ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var archive = testDir.ToFile(@"ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "notexists.zip");
                var mockTargetDir = testDir.ToDir("ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "Mock");
                //act
                Action action = () => _sevenZip.ExtractToDirectory(archive.FullName(), mockTargetDir.FullName());
                action.ShouldThrow<IOException>().And.Message.Should().StartWith("Archive not found:");
            });
        }

        [Fact]
        public void ExtractToDirectory_TargetDirAlreadyExists_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var targetDir = testDir.ToDir("ExtractToDirectory_TargetDirAlreadyExists_ExceptionIsThrown", "Mock");
                targetDir.CreateIfNotExists();

                //act
                Action action = () => _sevenZip.ExtractToDirectory(_sourceArchive.FullName(), targetDir.FullName());
                action.ShouldThrow<IOException>().And.Message.Should().StartWith("Target dir already exists at:");

            });
        }

        [Fact]
        public void ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var targetDir = testDir.ToDir("out");
                var targetFile = targetDir.ToFile("myfile.txt");
                targetDir.DeleteIfExists();
                targetDir.Exists().Should().BeFalse("arrange");
                targetFile.Exists().Should().BeFalse("arrange");

                //act

                var result = _sevenZip.ExtractToDirectory(_sourceArchive.FullName(), targetDir.FullName());
                Output.WriteLine($"ExtractToDirectory input: {result.Input}");
                Output.WriteLine($"ExtractToDirectory result: {result.Output}");
                targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");
                targetDir.ToDir().EnumeratePaths().Count().Should().Be(1);
                targetFile.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");

                var content = targetFile.ReadAllText();

                content.Should().Be("dummyContent");
            });
        }
    }
}
