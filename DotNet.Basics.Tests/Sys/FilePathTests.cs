using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FilePathTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testFile = @"\testc\file.txt";

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

        [Fact]
        public void ToFile_CombineToFileInfo_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToFile(_testFile).FullPath();
            const string expected = _testDirRoot + _testFile;
            actual.Should().Be(expected);
        }

        [Fact]
        public void ToFile_ParentFolderCombine_FileNameIsCombined()
        {
            var file = _testDoubleDir.ToFile(_testFile);
            file.FullPath().Should().EndWith(_testDoubleDir + _testFile);
        }
        
        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullPath().Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToFile(sourceFile.Name);

            targetfile.FullPath().Should().Be(@"c:\MyPath\myFile.temp");
        }
    }
}
