using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PathInfoTests
    {
        [Theory]
        [InlineData(@"c:\my\path", @"c:\my\path", PathSeparator.Backslash)]//bs to bs
        [InlineData(@"c:\my\path", @"c:/my/path", PathSeparator.Slash)]//bs to bs
        [InlineData(@"c:/my/path", @"c:/my/path", PathSeparator.Slash)]//s to  s
        [InlineData(@"c:/my/path", @"c:\my\path", PathSeparator.Backslash)]//s to bs
        public void RawPath_PathSeparator_SeparatorIsOverridden(string path, string expected, char separator)
        {
            //def path separator
            var pi = path.ToPath(separator);
            pi.RawPath.Should().Be(expected);
        }

        [Theory]
        //files
        [InlineData(@"c:\my\path", @"c:\my\path")]//absolute single path
        [InlineData(@"my\path", @"\my\path")]//relative single path with starting delimiter
        [InlineData(@"my\path", @"my\path")]//relative single path without starting delimiter
        //dirs
        [InlineData(@"c:\my\path\", @"c:\my\path\")]//absolute single path
        [InlineData(@"my\path\", @"\my\path\")]//relative single path with starting delimiter
        [InlineData(@"my\path\", @"my\path\")]//relative single path without starting delimiter
        //segments
        [InlineData(@"c:\my\path\", @"c:\my\", @"\path\")]//absolute segmented path
        [InlineData(@"my\path\", @"\my\", @"\path\")]//relative segmented path with starting delimiter
        [InlineData(@"my\path\", @"my\", @"\path\")]//relative segmented path without starting delimiter
        public void RawPath_RawPathParsing_RawPathIsParsed(string expected, string path, params string[] segments)
        {
            //def path separator
            var pi = path.ToPath(segments);
            pi.RawPath.Should().Be(expected);

            //alt path separator
            var altPi = path.ToPath(PathSeparator.Slash, segments);
            var altExpected = pi.RawPath.Replace(PathSeparator.Backslash, PathSeparator.Slash);

            altPi.RawPath.Should().Be(altExpected);
        }

        [Fact]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            const string path = "root";
            var root = path.ToPath();
            root.Add("sazas");//no change to original path
            root.RawPath.Should().Be(path);
        }

        [Theory]
        [InlineData("SomeDir\\MyFile.txt", "MyFile.txt")]//has extension
        [InlineData("SomeDir\\MyFile", "MyFile")]//no extension
        [InlineData("SomeDir\\.txt", ".txt")]//only extension
        [InlineData(null, "")]//name is null
        public void Name_NameWithExtension_NameIsFound(string name, string expected)
        {
            var file = name.ToFile();
            file.Name.Should().Be(expected, nameof(file.Name));
        }


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
        [InlineData("myFolder/DetectDelimiter/", PathSeparator.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", PathSeparator.Backslash)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter\\", PathSeparator.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter//", PathSeparator.Backslash)]//delimiter detected
        [InlineData("DetectDelimiter", PathSeparator.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, char delimiter)
        {
            var path = pathInput.ToPath();
            path.Separator.Should().Be(delimiter, pathInput);
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
                formatted.Should().EndWith(path.Separator.ToString());
            else
                formatted.Should().NotEndWith(path.Separator.ToString());
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/")]
        [InlineData("myFolder\\DetectDelimiter\\")]
        public void ToString_Autodetect_DelimiterIsDetected(string pathInput)
        {
            var path = pathInput.ToPath();

            var pathWithSlash = path.ToString().Replace(PathSeparator.Backslash, PathSeparator.Slash);
            var pathWithBackSlash = path.ToString().Replace(PathSeparator.Slash, PathSeparator.Backslash);

            pathWithSlash.Should().Be(pathInput.Replace('\\', '/'), PathSeparator.Slash.ToString());
            pathWithBackSlash.Should().Be(pathInput.Replace('/', '\\'), PathSeparator.Backslash.ToString());
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData(@"c:\", null)]
        [InlineData(@"myFolder\", null)]
        [InlineData(@"myParent\myFolder\", @"myParent\")]
        [InlineData(@"myParent\myFile.txt", @"myParent\")]
        [InlineData(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [InlineData(@"c:\myParent\myFile.txt", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var found = path.Parent?.RawPath;
            found.Should().Be(expectedParent, folder);
        }

        [Theory]
        [InlineData(@"c:\myParent\myFolder\", true)]
        [InlineData(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = folder.ToPath();

            var dir = path.Directory();

            dir.FullPath().Should().Be(isFolder ? path.FullPath() : path.Parent.FullPath());
        }
    }
}

