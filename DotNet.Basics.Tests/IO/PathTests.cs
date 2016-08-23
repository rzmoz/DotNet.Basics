﻿using System;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        //relative dir
        [TestCase(".\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //absolute dir
        [TestCase("c:\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //relative file
        [TestCase(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        //absolute file
        [TestCase("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        public void FullName_LongPath_LongPathsAreSupported(string p)
        {
            var path = p.ToPath();

            var expectedP = p;
            if (expectedP.StartsWith("."))
                expectedP = ".".ToPath(path.IsFolder).Add(p.RemovePrefix(".\\")).FullName;

            path.FullName.Should().Be(expectedP);//no exceptions
        }

        [Test]
        public void FullName_SystemIoCompliance_RelativePathsAreResolvedToSame()
        {
            var relativePath = "myfile.txt";
            var systemIo = new System.IO.FileInfo(relativePath);
            var path = relativePath.ToPath();

            System.IO.Path.GetFullPath(path.FullName).Should().Be(systemIo.FullName);
        }
        [Test]

        [TestCase("http://localhost/myDir/")] //http dir
        [TestCase("http://localhost/myFile")] //http file
        [TestCase("https://localhost/myDir/")] //https dir
        [TestCase("https://localhost/myFile/")] //https file
        public void FullName_Uri_UrisDontGetFileSystemAppended(string uri)
        {
            var path = uri.ToPath();
            path.FullName.Should().Be(uri);

        }


        [Test]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            const string path = "root";
            var root = path.ToPath();
            root.Add("sazas");//no change to original path
            root.RawName.Should().Be(path);
        }
        
        [Test]
        [TestCase(@"myFolder\", null)]
        [TestCase(@"myParent\myFolder\", @"myParent\")]
        [TestCase(@"myParent\myFile", @"myParent\")]
        [TestCase(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [TestCase(@"c:\myParent\myFile", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var parent = path.Parent;

            parent.RawName.Should().Be(expectedParent??".".ToDir().FullName);
        }

        [Test]
        [TestCase(@"c:\myParent\myFolder\", true)]
        [TestCase(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = folder.ToPath(isFolder);

            var dir = path.Directory;

            dir.FullName.Should().Be(isFolder ? path.FullName : path.Parent.FullName);
        }

        [Test]
        [TestCase("myFolder\\", "dir\\", true)]//backslash all dirs
        [TestCase("myFolder\\", "file.txt", true)]//backslash dir remains when file added
        [TestCase("myfile", "dir//", false)]//slash file remains when dir added - should throw exception?
        [TestCase("myfile.txt", "file.txt", false)]//slash file remains when dir added - should throw exception?
        public void Add_KeepIsFolder_PathRemainsRegardlesOfSegmentsAdded(string root, string newSegment, bool expectedIsFolder)
        {
            var path = root.ToPath();

            //assert before add
            path.IsFolder.Should().Be(expectedIsFolder);

            //act
            path = path.Add(newSegment);

            //assert
            path.IsFolder.Should().Be(expectedIsFolder);
        }

        [Test]
        [TestCase("myFolder", "")]//empty
        [TestCase("myFolder", null)]//null
        [TestCase("myFolder", "  ")]//spaces
        public void Add_EmptySegments_PathIsUnchanged(string root, string newSegment)
        {
            var path = root.ToPath().Add(newSegment);

            //assert
            path.RawName.Should().Be(root);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "", true)]//folder
        [TestCase("myFolder\\myFolder", "", false)]//folder without extension
        [TestCase("myFile.txt", ".txt", false)]//file with extension
        public void Extensions_Parsing_ExtensionIsParsed(string name, string extension, bool isFolder)
        {
            var path = name.ToPath(isFolder);
            //assert
            path.Extension.Should().Be(extension);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile", "myFile", false)]//file without extension
        [TestCase("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void RawName_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);

            if (isFolder)
                fullPath = fullPath.EnsureSuffix(path.Delimiter.ToChar());

            path.RawName.Should().Be(fullPath);
        }
        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile", "myFile", false)]//file without extension
        [TestCase("myFolder\\myFile.txt", "myFile", false)]//file with extension
        public void NameWithoutExtension_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);
            //assert
            path.NameWithoutExtension.Should().Be(expectedName);
        }
        
        [Test]
        [TestCase("myFolder/DetectDelimiter/", PathDelimiter.Slash)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter\\", PathDelimiter.Backslash)]//delimiter detected
        [TestCase("myFolder/DetectDelimiter\\", PathDelimiter.Slash)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter//", PathDelimiter.Backslash)]//delimiter detected
        [TestCase("DetectDelimiter", PathDelimiter.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, PathDelimiter delimiter)
        {
            var path = pathInput.ToPath();
            path.Delimiter.Should().Be(delimiter);
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/", true)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter\\", true)]//delimiter detected
        [TestCase("myFolder/DetectDelimiter", false)]//delimiter fallback
        public void IsFolder_Detection_IsFolderIsDetected(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath();
            path.IsFolder.Should().Be(isFolder);
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/", true)]//folder with delimiter in the end
        [TestCase("myFolder/DetectDelimiter", true)]//folder withouth delimiter in the end
        [TestCase("myFolder/myFile.txt", false)]//delimiter fallback
        public void IsFolder_Formatting_FolderExtensionIsOutput(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath(isFolder);
            path.IsFolder.Should().Be(isFolder);
            var formatted = path.ToString();
            if (isFolder)
                formatted.Should().EndWith(path.Delimiter.ToChar().ToString());
            else
                formatted.Should().NotEndWith(path.Delimiter.ToChar().ToString());
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/")]
        [TestCase("myFolder\\DetectDelimiter\\")]
        public void ToString_Autodetect_DelimiterIsDetected(string pathInput)
        {
            var path = pathInput.ToPath();

            var pathWithSlash = path.ToString(PathDelimiter.Slash);
            var pathWithBackSlash = path.ToString(PathDelimiter.Backslash);

            pathWithSlash.Should().Be(pathInput.Replace('\\', '/'), PathDelimiter.Slash.ToName());
            pathWithBackSlash.Should().Be(pathInput.Replace('/', '\\'), PathDelimiter.Backslash.ToName());
        }
    }
}
