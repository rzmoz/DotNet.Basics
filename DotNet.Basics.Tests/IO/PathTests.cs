using System;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    
    public class PathTests
    {
        [Theory]
        //relative dir
        [InlineData(".\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //absolute dir
        [InlineData("c:\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //relative file
        [InlineData(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        //absolute file
        [InlineData("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        public void FullName_LongPath_LongPathsAreSupported(string p)
        {
            var path = p.ToPath();

            var expectedP = p;
            if (expectedP.StartsWith("."))
                expectedP = ".".ToPath(path.IsFolder).Add(p.RemovePrefix(".\\")).FullName;

            path.FullName.Should().Be(expectedP);//no exceptions
        }

        [Fact]
        public void FullName_SystemIoCompliance_RelativePathsAreResolvedToSame()
        {
            var relativePath = "myfile.txt";
            var systemIo = new System.IO.FileInfo(relativePath);
            var path = relativePath.ToPath();

            System.IO.Path.GetFullPath(path.FullName).Should().Be(systemIo.FullName);
        }

        [Theory]
        [InlineData("http://localhost/myDir/")] //http dir
        [InlineData("http://localhost/myFile")] //http file
        [InlineData("https://localhost/myDir/")] //https dir
        [InlineData("https://localhost/myFile/")] //https file
        public void FullName_Uri_UrisDontGetFileSystemAppended(string uri)
        {
            var path = uri.ToPath();
            path.FullName.Should().Be(uri);
        }

        [Theory]
        [InlineData("http://localhost/", "myDir/")] //http dir
        [InlineData("http://localhost/", "myFile")] //http file
        [InlineData("https://localhost/", "myDir/")] //https dir
        [InlineData("https://localhost/", "myFile")] //https file
        public void FullName_Uri_ParsedPathIsUri(string uri,string segments)
        {
            var path = uri.ToPath(segments);
            Action action = ()=> new Uri(path.FullName);
            action.ShouldNotThrow<UriFormatException>();
        }

        [Fact]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            const string path = "root";
            var root = path.ToPath();
            root.Add("sazas");//no change to original path
            root.RawName.Should().Be(path);
        }
        
        [Theory]
        [InlineData(@"myFolder\", null)]
        [InlineData(@"myParent\myFolder\", @"myParent\")]
        [InlineData(@"myParent\myFile", @"myParent\")]
        [InlineData(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [InlineData(@"c:\myParent\myFile", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var parent = path.Parent;

            parent.RawName.Should().Be(expectedParent??".".ToDir().FullName);
        }

        [Theory]
        [InlineData(@"c:\myParent\myFolder\", true)]
        [InlineData(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = folder.ToPath(isFolder);

            var dir = path.Directory;

            dir.FullName.Should().Be(isFolder ? path.FullName : path.Parent.FullName);
        }

        [Theory]
        [InlineData("myFolder\\", "dir\\", true)]//backslash all dirs
        [InlineData("myFolder\\", "file.txt", true)]//backslash dir remains when file added
        [InlineData("myfile", "dir//", false)]//slash file remains when dir added - should throw exception?
        [InlineData("myfile.txt", "file.txt", false)]//slash file remains when dir added - should throw exception?
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

        [Theory]
        [InlineData("myFolder", "")]//empty
        [InlineData("myFolder", null)]//null
        [InlineData("myFolder", "  ")]//spaces
        public void Add_EmptySegments_PathIsUnchanged(string root, string newSegment)
        {
            var path = root.ToPath().Add(newSegment);

            //assert
            path.RawName.Should().Be(root);
        }

        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [InlineData("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("myFolder\\myFolder\\", "", true)]//folder
        [InlineData("myFolder\\myFolder", "", false)]//folder without extension
        [InlineData("myFile.txt", ".txt", false)]//file with extension
        public void Extensions_Parsing_ExtensionIsParsed(string name, string extension, bool isFolder)
        {
            var path = name.ToPath(isFolder);
            //assert
            path.Extension.Should().Be(extension);
        }

        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [InlineData("myFolder\\myFile", "myFile", false)]//file without extension
        [InlineData("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void RawName_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);

            if (isFolder)
                fullPath = fullPath.EnsureSuffix(path.Delimiter.ToChar());

            path.RawName.Should().Be(fullPath);
        }
        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [InlineData("myFolder\\myFile", "myFile", false)]//file without extension
        [InlineData("myFolder\\myFile.txt", "myFile", false)]//file with extension
        public void NameWithoutExtension_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath(isFolder);
            //assert
            path.NameWithoutExtension.Should().Be(expectedName);
        }
        
        [Theory]
        [InlineData("myFolder/DetectDelimiter/", PathDelimiter.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", PathDelimiter.Backslash)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter\\", PathDelimiter.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter//", PathDelimiter.Backslash)]//delimiter detected
        [InlineData("DetectDelimiter", PathDelimiter.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, PathDelimiter delimiter)
        {
            var path = pathInput.ToPath();
            path.Delimiter.Should().Be(delimiter);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", true)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", true)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter", false)]//delimiter fallback
        public void IsFolder_Detection_IsFolderIsDetected(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath();
            path.IsFolder.Should().Be(isFolder);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", true)]//folder with delimiter in the end
        [InlineData("myFolder/DetectDelimiter", true)]//folder withouth delimiter in the end
        [InlineData("myFolder/myFile.txt", false)]//delimiter fallback
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

        [Theory]
        [InlineData("myFolder/DetectDelimiter/")]
        [InlineData("myFolder\\DetectDelimiter\\")]
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
