using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PathInfoTests
    {
        [Theory]
        [InlineData("/is_rooted/", true)] //mac / unix
        [InlineData("/is_rooted", true)] //mac / unix
        public void Path_IsPathRooted_SupportsAllPlatforms(string path, bool expected)
        {
            Path.IsPathRooted(path).Should().Be(expected);
        }

        [Theory]
        [InlineData(@"http://localhost/dir")]
        public void RawPath_SlashSeparator_SeparatorIsKept(string path)
        {
            //def path separator
            var pi = path.ToPath();
            pi.RawPath.Should().Be(path);
        }

        [Fact]
        public void UriScheme_Detection_SchemeIsKept()
        {
            var uriScheme = "http://";
            var host = "localhost.com/";
            var subPath = "my/dir";
            var url = uriScheme.ToPath(host, subPath);

            url.RawPath.Should().Be($"{uriScheme}{host}{subPath}");
        }

        [Fact]
        public void Directory_TypeIsFile_DirIsParent()
        {
            var file = "myParent\\my.file.txt".ToFile();

            file.Directory.Should().Be(file.Parent);
        }

        [Fact]
        public void Directory_TypeIsDir_DirIsSame()
        {
            var dir = "myParent\\my.dir".ToDir();

            dir.Directory.Should().Be(dir);
        }

        [Fact]
        public void Parent_NotRootedWithParent_ParentIsReturned()
        {
            var dir = "myParent\\myDir".ToDir();

            var parent = dir.Parent;

            parent.Should().Be("myParent".ToDir());
        }

        [Fact]
        public void Parent_NotRootedNoParent_NullIsReturned()
        {
            var dir = "myDir".ToDir();

            var parent = dir.Parent;

            parent.Should().BeNull();
        }

        [Theory]
        [InlineData(@"c:\my\path", @"c:/my/path")] //bs to bs
        [InlineData(@"c:/my/path", @"c:/my/path")] //s to  s
        public void RawPath_PathSeparator_SeparatorIsConformed(string path, string expected)
        {
            //def path separator
            var pi = path.ToPath();
            pi.RawPath.Should().Be(expected);
        }

        [Theory]
        //files
        [InlineData(@"c:/my/path", @"c:/my/path")] //absolute single path
        [InlineData(@"/my/path", @"/my/path")] //relative single path with starting delimiter
        [InlineData(@"my/path", @"my/path")] //relative single path without starting delimiter
        //dirs
        [InlineData(@"c:/my/path", @"c:/my/path/")] //absolute single path
        [InlineData(@"/my/path", @"/my/path/")] //relative single path with starting delimiter
        [InlineData(@"my/path", @"my/path/")] //relative single path without starting delimiter
        //segments
        [InlineData(@"c:/my/path", @"c:/my/", @"/path/")] //absolute segmented path
        [InlineData(@"/my/path", @"/my/", @"/path/")] //relative segmented path with starting delimiter
        [InlineData(@"my/path", @"my/", @"/path/")] //relative segmented path without starting delimiter
        public void RawPath_RawPathParsing_RawPathIsParsed(string expected, string path, params string[] segments)
        {
            //def path separator
            var pi = path.ToPath(segments);
            pi.RawPath.Should().Be(expected);

            //alt path separator
            var altPi = path.ToPath(segments);
            altPi.RawPath.Should().Be(pi.RawPath);
        }

        [Theory]
        [InlineData("SomeDir/MyFile.txt", "MyFile.txt")] //has extension
        [InlineData("SomeDir/MyFile", "MyFile")] //no extension
        [InlineData("SomeDir/.txt", ".txt")] //only extension
        public void Name_NameWithExtension_NameIsFound(string name, string expected)
        {
            var file = name.ToFile();
            file.Name.Should().Be(expected, nameof(file.Name));
        }

        [Theory]
        [InlineData("myFolder/myFolder/", "myFolder")] //folder with trailing delimiter
        [InlineData("myFolder/myFolder", "myFolder")] //folder without trailing delimiter
        [InlineData("myFolder/myFile.txt", "myFile.txt")] //file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName)
        {
            var path = fullPath.ToPath();
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Theory]
        //[InlineData("myFolder/DetectDelimiter/", PathType.Dir)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", PathType.Dir)] //delimiter detected
        //[InlineData("myFolder/DetectDelimiter", PathType.File)]//delimiter fallback
        public void IsFolder_Detection_IsFolderIsDetected(string pathInput, PathType pathType)
        {
            var path = pathInput.ToPath();
            path.PathType.Should().Be(pathType);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", PathType.Dir)] //folder with delimiter in the end
        [InlineData("myFolder\\DetectDelimiter", PathType.File)] //folder withouth delimiter in the end
        [InlineData("myFolder/myFile.txt", PathType.File)] //delimiter fallback
        public void IsFolder_Formatting_FolderExtensionIsOutput(string pathInput, PathType pathType)
        {
            var path = pathInput.ToPath();
            path.PathType.Should().Be(pathType);
            var formatted = path.ToString();
            formatted.Should().NotEndWith("/");
            formatted.Contains('/').Should().BeTrue();
        }

        [Theory]
        [InlineData(@"c:\", null)]
        [InlineData(@"myFolder\", null)]
        [InlineData(@"myParent\myFolder\", @"myParent")]
        [InlineData(@"myParent\myFile.txt", @"myParent")]
        [InlineData(@"/myParent\myFolder\", @"/myParent")]
        [InlineData(@"/myParent\myFile.txt", @"/myParent")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var found = path.Parent?.RawPath;
            found.Should().Be(expectedParent, folder);
        }

        [Theory]
        [InlineData(@"c:\hello", new[] { @"my\folder", "myFile.txt" }, 5)]
        //[InlineData(null, new[] { @"c:", "my", "folder" }, 3)]
        public void Tokenize_CleanSegments_SegmentsAreCombinedAndCleaned(string path, string[] segments,
            int expectedNumOfSegments)
        {
            var flattened = PathInfo.Flatten(path, segments);
            var splitSegments = PathInfo.Tokenize(flattened);
            splitSegments.Count.Should().Be(expectedNumOfSegments);
        }
    }
}