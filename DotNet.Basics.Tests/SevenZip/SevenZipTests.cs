using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNet.Basics.SevenZip;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.SevenZip
{
    public class SevenZipTests : TestWithHelpers
    {
        private SevenZipExe _sevenZip;
        private FilePath _sourceArchive;

        public SevenZipTests(ITestOutputHelper output) : base(output)
        {
            WithTestRoot(testRoot =>
            {
                _sevenZip = new SevenZipExe(testRoot, o => Output.WriteLine(o));
                _sourceArchive = testRoot.ToFile("SevenZip", "myArchive.zip");
            });
        }

        [Fact]
        public void CreateFromDirectory_DontOverWrite_ExceptionWhenTargetAlreadyExists()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var targetPath = testDir.ToFile("SevenZip", "myArchive.zip");
                targetPath.DeleteIfExists();
                targetPath.WriteAllText("dummyContent");
                targetPath.Exists().Should().BeTrue();

                //act
                Action action = () => _sevenZip.CreateZipFromDirectory("mySource", targetPath.FullName(), false);

                action.Should().Throw<IOException>();
            });

        }

        [Fact]
        public void CreateFromDirectory_Zip_ContentIsZipped()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var sourceDir = testDir.ToDir("CreateFromDirectory_Zip_ContentIsZipped", "source");
                var dummyFile = sourceDir.ToFile("myFile.txt");
                var dummyContent = "dummyContent";

                dummyFile.WriteAllText(dummyContent);

                var targetArchive = testDir.ToFile("CreateFromDirectory_Zip_ContentIsZipped", "myArchive.zip");
                targetArchive.DeleteIfExists();

                targetArchive.Exists().Should().BeFalse();

                //act
                _sevenZip.CreateZipFromDirectory(sourceDir.FullName(), targetArchive.FullName());

                targetArchive.Exists().Should().BeTrue($"Exists:{targetArchive.FullName()}");
                using (var archive = ZipFile.OpenRead(targetArchive.FullName()))
                {
                    archive.Entries.Count.Should().Be(1);

                    using (var stream = archive.Entries.Single().Open())
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var read = reader.ReadToEnd();
                        read.Should().Be(dummyContent);
                    }
                }
            });
        }

        [Fact]
        public void CreateFromDirectory_7z_ContentIsZipped()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var sourceDir = testDir.ToDir("CreateFromDirectory_7z_ContentIsZipped", "source");
                var dummyFile = sourceDir.ToFile("myFile.txt");
                var dummyContent = "dummyContent";

                dummyFile.WriteAllText(dummyContent);

                var targetArchive = testDir.ToFile("CreateFromDirectory_7z_ContentIsZipped", "myArchive.7z");
                targetArchive.DeleteIfExists();

                targetArchive.Exists().Should().BeFalse();

                //act
                _sevenZip.Create7zFromDirectory(sourceDir.FullName(), targetArchive.FullName());

                targetArchive.Exists().Should().BeTrue($"Exists:{targetArchive.FullName()}");
            });
        }

        [Fact]
        public void ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var archive = testDir.ToFile(@"ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "notExists.zip");
                var mockTargetDir = testDir.ToDir("ExtractToDirectory_ArchiveNotFound_ExceptionIsThrown", "Mock");
                //act
                Action action = () => _sevenZip.ExtractToDirectory(archive.FullName(), mockTargetDir.FullName());
                action.Should().Throw<IOException>().And.Message.Should().StartWith("Archive not found:");
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
                action.Should().Throw<IOException>().And.Message.Should().StartWith("Target dir already exists at:");

            });
        }

        [Fact]
        public void ExtractToDirectory_TargetDirDoesntExist_ArchiveIsExtractedToNewDir()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var targetDir = testDir.ToDir("out");
                var targetFile = targetDir.ToFile("myFile.txt");
                targetDir.DeleteIfExists();
                targetDir.Exists().Should().BeFalse("arrange");
                targetFile.Exists().Should().BeFalse("arrange");

                //act

                var exitCode = _sevenZip.ExtractToDirectory(_sourceArchive.FullName(), targetDir.FullName());
                Output.WriteLine($"ExtractToDirectory input: {exitCode}");
                targetDir.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");
                targetDir.ToDir().EnumeratePaths().Count().Should().Be(1);
                targetFile.Exists().Should().BeTrue($"Exists:{targetDir.FullName()}");

                var content = targetFile.ReadAllText();

                content.Should().Be("dummyContent");
            });
        }
    }
}
