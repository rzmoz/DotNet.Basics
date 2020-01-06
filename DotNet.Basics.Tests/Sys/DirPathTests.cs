using System;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class DirPathTests
    {
        private const string _path = "c:/mypath";
        private const string _segment = "segment";

        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testSingleDir = @"\testk\";

        public DirPath DirPath { get; set; }//Used for deserialization test

        [Fact]
        public void ExplicitCast_FromString_StringIsCast()
        {
            var dirStr = "lorem/ipsum/";
            var dirPath = (DirPath)dirStr;
            dirPath.RawPath.Should().Be(dirStr);
        }

        [Fact]
        public void Serialize_SystemTextJson_DirPathIsSerialized()
        {
            var dirStr = "lorem/ipsum/".ToDir();

            Action action = () => System.Text.Json.JsonSerializer.Serialize(dirStr);

            action.Should().NotThrow();
        }

        [Fact]
        public void Serialize_NewtonSoft_StringIsSerialized()
        {
            var dirStr = "lorem/ipsum/".ToDir();

            Action action = () => JsonConvert.SerializeObject(dirStr);

            action.Should().NotThrow();
        }

        [Fact]
        public void Deserialization_StringToDirPath_StringIsDeserializedToDirPath()
        {
            var dirStr = "lorem/ipsum/";
            var obj = JsonConvert.DeserializeObject<DirPathTests>($"{{'{nameof(DirPath)}':'{dirStr}'}}");
            obj.DirPath.RawPath.Should().Be(dirStr);
        }

        [Fact]
        public void Add_Dir_SameTypeIsReturned()
        {
            var dir = _path.ToDir().Add(_segment);

            dir.Should().BeOfType<DirPath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }/");
        }

        [Fact]
        public void ToDir_Create_DirIsCreated()
        {
            var dir = _path.ToDir().ToFile().ToDir(_segment);//different extension methods

            dir.Should().BeOfType<DirPath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }/");
        }

        [Theory]
        [InlineData("SomeDir\\MyDir.txt", "MyDir")]//has extension
        [InlineData("SomeDir\\MyDir", "MyDir")]//no extension
        [InlineData("SomeDir\\.txt", "")]//only extension
        public void NameWoExtension_WithoutExtension_NameIsRight(string name, string nameWoExtensions)
        {
            var file = name.ToDir();
            file.NameWoExtension.Should().Be(nameWoExtensions);
        }

        [Theory]
        [InlineData("SomeDir\\MyDir.txt", ".txt")]//has extension
        [InlineData("SomeDir\\MyDir", "")]//no extension
        [InlineData("SomeDir\\.txt", ".txt")]//only extension
        public void Extension_Extension_ExtensionsIsRight(string name, string extension)
        {
            var file = name.ToDir();
            file.Extension.Should().Be(extension);
        }

        [Fact]
        public void ToDir_ParentFromCombine_ParentFolderOfNewIsSameAsOrgRoot()
        {
            var rootDir = _testDirRoot.ToDir();
            var dir = rootDir.ToDir(_testSingleDir);
            dir.Parent.Name.Should().Be(rootDir.Name);
        }
    }
}
