using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Extensions.SevenZip.Tests
{
    public class ZipReaderTests:TestWithHelpers
    {
        private readonly FilePath _testSource;

        public ZipReaderTests(ITestOutputHelper output) : base(output)
        {
            _testSource = TestRoot.ToFile("NewFolder.zip");
            _testSource.Exists().Should().BeTrue(_testSource.FullName(), true);
        }
        
        [Theory]
        [InlineData("myfile.txt", false)]//file in root not found
        [InlineData("mydir/", false)]//explicit dir in root not found
        [InlineData("mydir", false)]//implicit dir in root not found
        [InlineData("New folder (1)/myfile.txt", false)]//file in sub dir not found
        [InlineData("New folder (1)/mydir/", false)]//explicit dir in sub dir not found
        [InlineData("New folder (1)/mydir", false)]//implicit dir in sub dir not found

        [InlineData("New Doc (1).txt", true)]//file in root found
        [InlineData("New Folder (1)/", true)]//explicit dir in root found
        [InlineData("New Folder (1)", true)]//implicit dir in root found
        [InlineData("New folder (1)/New Doc (1) In New Folder (1).txt", true)]//file in sub dir found
        [InlineData("New folder (1)/New folder (1) In New Folder (1)/", true)]//explicit dir in sub dir found
        [InlineData("New folder (1)/New folder (1) In New Folder (1)", true)]//implicit dir in sub dir found
        public void HasEntry_Entries_EntriesAreMatched(string path, bool expected)
        {
            using (var reader = new ZipReader(_testSource))
            {
                reader.HasEntry(path).Should().Be(expected, path);
            }
        }
    }
}
