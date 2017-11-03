using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FilePathTests
    {
        private const string _path = "c:/mypath";
        private const string _segment = "segment";
        
        [Fact]
        public void Add_File_SameTypeIsReturned()
        {
            var dir = _path.ToFile().Add(_segment);

            dir.Should().BeOfType<FilePath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }");
        }
        [Fact]
        public void ToFile_Create_FileIsCreated()
        {
            var file = _path.ToFile().ToDir().ToFile(_segment);//different extension methods

            file.Should().BeOfType<FilePath>();
            file.RawPath.Should().Be(_path + $"/{_segment }");
        }
        
        [Theory]
        [InlineData("SomeDir\\MyFile.txt", "MyFile")]//has extension
        [InlineData("SomeDir\\MyFile", "MyFile")]//no extension
        [InlineData("SomeDir\\.txt", "")]//only extension
        [InlineData(null, "")]//name is null
        public void NameWoExtension_WithoutExtension_NameIsRight(string name, string nameWoExtensions)
        {
            var file = name.ToFile();
            file.NameWoExtension.Should().Be(nameWoExtensions);
        }
        [Theory]
        [InlineData("SomeDir\\MyFile.txt", ".txt")]//has extension
        [InlineData("SomeDir\\MyFile", "")]//no extension
        [InlineData("SomeDir\\.txt", ".txt")]//only extension
        [InlineData(null, "")]//name is null
        public void Extension_Extension_ExtensionsIsRight(string name, string extension)
        {
            var file = name.ToFile();
            file.Extension.Should().Be(extension);
        }
    }
}
