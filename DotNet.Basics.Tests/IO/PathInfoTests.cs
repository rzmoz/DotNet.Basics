using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class PathInfoTests
    {
        [Fact]
        public void SplitSplitSegments_UriSUpport_UriIsSPlitProperly()
        {
            var segments = new[]
            {
                "http://localhost:80/",
                "myfile.aspx"
            };

            var pi = new PathInfo(segments);

            pi.Segments.Count.Should().Be(3);
            pi.Segments.First().Should().Be("http://");
            pi.Segments.Skip(1).First().Should().Be("localhost:80");
            pi.Segments.Skip(2).First().Should().Be("myfile.aspx");
        }

        [Fact]
        public void Ctor_IsUri_DelimiterIsAlwaysSlash()
        {
            var path = "http://localhost\\mydir";
            var delimiter = PathDelimiter.Slash;

            var sp = new PathInfo(path, true, delimiter);

            sp.Segments.Count.Should().Be(3);
            sp.IsFolder.Should().BeTrue(nameof(sp.IsFolder));
            sp.Delimiter.Should().Be(PathDelimiter.Slash);
        }

        [Fact]
        public void Ctor_Properties_PropertiesAreSet()
        {
            var path = "sdfsersfsf";
            var segments = new[] { "sdfsersfsf", "sdfsersfsf", "sdfsersfsf", "sdfsersfsf" };
            var isFolder = true;
            var delimiter = PathDelimiter.Slash;

            var sp = new PathInfo(path, segments, isFolder, delimiter);

            sp.Segments.Count.Should().Be(segments.Length + 1);
            sp.Segments.All(s => s == path).Should().BeTrue();
            sp.IsFolder.Should().Be(isFolder);
            sp.Delimiter.Should().Be(delimiter);

        }

        [Theory]
        [InlineData("myFolder\\myFolder/", PathDelimiter.Backslash)]//mixed delimiters - first is picked
        [InlineData("myFolder\\myFolder", PathDelimiter.Backslash)]//backslash
        [InlineData("myFolder/myFolder", PathDelimiter.Slash)]//slash
        [InlineData("myFolder/myFolder\\", PathDelimiter.Slash)]//mixed delimiters - first is picked
        [InlineData("myFolder", PathDelimiter.Backslash)]//default fallback
        public void Ctor_DelimiterDetection_DelimiterIsDetected(string path, char expected)
        {
            var sp = new PathInfo(path);
            sp.Delimiter.Should().Be(expected, path);
        }

        [Theory]
        [InlineData("myFolder\\myFolder/", true)]//folder with trailing slash
        [InlineData("myFolder\\myFolder\\", true)]//folder with trailing backslash
        [InlineData("myFolder\\myFile", false)]//file without extension
        [InlineData("myFolder\\myFile.txt", false)]//file with extension
        public void Ctor_IsFolderDetection_FolderIsDetected(string path, bool expected)
        {
            var sp = new PathInfo(path);
            sp.IsFolder.Should().Be(expected);
        }
        
        [Fact]
        public void Ctor_PathInSegments_PathsAreParsed()
        {
            var pathInSegment = "dir/dir/dir/dir/dir/dir/dir";
            var sp = new PathInfo(pathInSegment, pathInSegment);
            sp.Segments.Count.Should().Be(14);
            sp.Segments.Any(s => s.Contains(PathDelimiter.Slash)).Should().BeFalse();
            sp.Segments.All(s => s == "dir").Should().BeTrue();
        }

        [Fact]
        public void Serialization_JsonSerialization_PathIsSerializedButNotDeserialized()
        {
            var path = "MyPath";

            var sp = new PathInfo(path, path, path, path);

            //act
            string serialized = JsonConvert.SerializeObject(sp);
            //assert
            serialized.Should().Be(@"{""Name"":""MyPath"",""IsFolder"":false,""Delimiter"":""\\"",""Segments"":[""MyPath"",""MyPath"",""MyPath"",""MyPath""]}");
        }
    }
}
