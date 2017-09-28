using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class PathTests
    {
        /*MOVED
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

            var expectedP = SystemIoPath.GetFullPath(p);

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

        [Fact]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            const string path = "root";
            var root = path.ToPath();
            root.Add("sazas");//no change to original path
            root.RawName.Should().Be(path);
        }
        
        [Theory]
        [InlineData(@"c:\", null)]
        [InlineData(@"myFolder\", null)]
        [InlineData(@"myParent\myFolder\", @"myParent\")]
        [InlineData(@"myParent\myFile", @"myParent\")]
        [InlineData(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [InlineData(@"c:\myParent\myFile", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var found = path.Parent?.RawName;
            found.Should().Be(expectedParent, folder);
        }

        [Theory]
        [InlineData(@"c:\myParent\myFolder\", true)]
        [InlineData(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = folder.ToPath();

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
        */
        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder")]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder")]//folder without trailing delimiter
        [InlineData("myFolder\\myFile.txt", "myFile.txt")]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName)
        {
            var path = fullPath.ToPath();
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("myFolder\\myFolder\\", "")]//folder
        [InlineData("myFolder\\myFolder", "")]//folder without extension
        [InlineData("myFile.txt", ".txt")]//file with extension
        public void Extensions_Parsing_ExtensionIsParsed(string name, string extension)
        {
            var path = name.ToPath();
            //assert
            path.Extension.Should().Be(extension);
        }

        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder", false)]//folder without trailing delimiter
        [InlineData("myFolder\\myFile", "myFile", false)]//file without extension
        [InlineData("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void RawName_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath();

            if (isFolder)
                fullPath = fullPath.EnsureSuffix(path.Delimiter);

            path.RawName.Should().Be(fullPath);
        }
        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [InlineData("myFolder\\myFile", "myFile", false)]//file without extension
        [InlineData("myFolder\\myFile.txt", "myFile", false)]//file with extension
        public void NameWithoutExtension_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = fullPath.ToPath();
            //assert
            path.NameWithoutExtension.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", PathDelimiter.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", PathDelimiter.Backslash)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter\\", PathDelimiter.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter//", PathDelimiter.Backslash)]//delimiter detected
        [InlineData("DetectDelimiter", PathDelimiter.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, char delimiter)
        {
            var path = pathInput.ToPath();
            path.Delimiter.Should().Be(delimiter, pathInput);
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
        [InlineData("myFolder/DetectDelimiter", false)]//folder withouth delimiter in the end
        [InlineData("myFolder/myFile.txt", false)]//delimiter fallback
        public void IsFolder_Formatting_FolderExtensionIsOutput(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath();
            path.IsFolder.Should().Be(isFolder);
            var formatted = path.ToString();
            if (isFolder)
                formatted.Should().EndWith(path.Delimiter.ToString());
            else
                formatted.Should().NotEndWith(path.Delimiter.ToString());
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/")]
        [InlineData("myFolder\\DetectDelimiter\\")]
        public void ToString_Autodetect_DelimiterIsDetected(string pathInput)
        {
            var path = pathInput.ToPath();

            var pathWithSlash = path.ToString(PathDelimiter.Slash);
            var pathWithBackSlash = path.ToString(PathDelimiter.Backslash);

            pathWithSlash.Should().Be(pathInput.Replace('\\', '/'), PathDelimiter.Slash.ToString());
            pathWithBackSlash.Should().Be(pathInput.Replace('/', '\\'), PathDelimiter.Backslash.ToString());
        }
    }
}
