using DotNet.Basics.Compression;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Compression
{
    [TestFixture]
    public class ZipReaderTests
    {
        private FilePath _testSource;

        [SetUp]
        public void SetUp()
        {
            var archiveSource = TestContext.CurrentContext.TestDirectory.ToFile("Compression", "NewFolder.zip");
            _testSource = TestContext.CurrentContext.TestDirectory.ToFile("HasEntry_Entries_EntriesAreMatched", archiveSource.Name);
            archiveSource.CopyTo(_testSource, overwrite: true);

            _testSource.Exists().Should().BeTrue(_testSource.FullName, true);
        }


        [Test]
        [TestCase("myfile.txt")]//file in root not found
        [TestCase("mydir/")]//explicit dir in root not found
        [TestCase("mydir")]//implicit dir in root not found
        [TestCase("New folder (1)/myfile.txt")]//file in sub dir not found
        [TestCase("New folder (1)/mydir/")]//explicit dir in sub dir not found
        [TestCase("New folder (1)/mydir")]//implicit dir in sub dir not found

        [TestCase("New Doc (1).txt", true)]//file in root found
        [TestCase("New Folder (1)/", true)]//explicit dir in root found
        [TestCase("New Folder (1)", true)]//implicit dir in root found
        [TestCase("New folder (1)/New Doc (1) In New Folder (1).txt", true)]//file in sub dir found
        [TestCase("New folder (1)/New folder (1) In New Folder (1)/", true)]//explicit dir in sub dir found
        [TestCase("New folder (1)/New folder (1) In New Folder (1)", true)]//implicit dir in sub dir found
        public void HasEntry_Entries_EntriesAreMatched(string path, bool expected = false)
        {
            using (var reader = new ZipReader(_testSource))
            {
                reader.HasEntry(path).Should().Be(expected, path);
            }
        }
    }
}
