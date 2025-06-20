﻿using System;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FilePathTests
    {
        private const string _path = "/mypath";
        private const string _segment = "segment";

        public FilePath FilePath { get; set; }//Used for deserialization test

        [Fact]
        public void ImplicitCast_FromString_StringIsCastToFilePath()
        {
            var fileStr = "lorem/ipsum";
            FilePath filePath = fileStr;//compiles
            filePath.RawPath.Should().Be(fileStr);
            fileStr.Should().Be(filePath);
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
            var fileStr = "/lorem/ipsum";
            var obj = JsonConvert.DeserializeObject<FilePathTests>($"{{'{nameof(FilePath)}':'{fileStr}'}}");
            obj.FilePath.RawPath.Should().Be(fileStr);
        }
        
        [Fact]
        public void ToFile_Create_FileIsCreated()
        {
            var file = _path.ToFile().ToDir().ToFile(_segment);//different extension methods

            file.Should().BeOfType<FilePath>();
            file.RawPath.Should().Be(_path + $"/{_segment }");
        }

        [Theory]
        [InlineData("SomeDir/MyFile.txt", "MyFile")]//has extension
        [InlineData("SomeDir/MyFile", "MyFile")]//no extension
        [InlineData("SomeDir/.txt", "")]//only extension
        public void NameWoExtension_WithoutExtension_NameIsRight(string name, string nameWoExtensions)
        {
            var file = name.ToFile();
            file.NameWoExtension.Should().Be(nameWoExtensions);
        }

        [Theory]
        [InlineData("SomeDir/MyFile.txt", ".txt")]//has extension
        [InlineData("SomeDir/MyFile", "")]//no extension
        [InlineData("SomeDir/.txt", ".txt")]//only extension
        public void Extension_Extension_ExtensionsIsRight(string name, string extension)
        {
            var file = name.ToFile();
            file.Extension.Should().Be(extension);
        }
    }
}
