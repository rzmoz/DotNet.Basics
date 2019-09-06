using System;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FilePathTests
    {
        private const string _path = "c:/mypath";
        private const string _segment = "segment";

        public FilePath FilePath { get; set; }//Used for deserialization test

        [Fact]
        public void ExplicitCast_FromString_StringIsCastToFilePath()
        {
            var fileStr = "lorem/ipsum";
            var filePath = (FilePath)fileStr;
            filePath.RawPath.Should().Be(fileStr);
        }

        [Fact]
        public void Serialize_SystemTextJson_DirPathIsSerialized()
        {
            var fileStr = "lorem/ipsum/myfile.txt".ToFile();

            Action action = () => System.Text.Json.JsonSerializer.Serialize(fileStr);

            action.Should().NotThrow();
        }

        [Fact]
        public void Serialize_NewtonSoft_DirPathIsSerialized()
        {
            var fileStr = "lorem/ipsum/myfile.txt".ToFile();

            Action action = () => JsonConvert.SerializeObject(fileStr);

            action.Should().NotThrow();
        }

        [Fact]
        public void Deserialization_StringToFilPath_StringIsDeserialized()
        {
            var fileStr = "lorem/ipsum";
            var obj = JsonConvert.DeserializeObject<FilePathTests>($"{{'{nameof(FilePath)}':'{fileStr}'}}");
            obj.FilePath.RawPath.Should().Be(fileStr);
        }

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
