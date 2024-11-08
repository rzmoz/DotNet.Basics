using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class PathInfoJsonConverterTests
    {
        [Fact]
        public void ConvertDir_Serialize_PathIsSerialized()
        {
            var rawPath = @"/my/dirPath/";
            var path = rawPath.ToPath();

            var json = path.ToJson();

            json.Should().Be($@"""{rawPath.TrimEnd('/')}""");
        }

        [Fact]
        public void ConvertFile_Serialize_PathIsSerialized()
        {
            var rawPath = @"/my/file.txt";
            var path = rawPath.ToPath();

            var json = path.ToJson();

            json.Should().Be($@"""{rawPath}""");
        }

        [Fact]
        public void ConvertDir_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"/my/path/";
            var path = $@"""{rawPath}""".FromJson<PathInfo>();

            path.RawPath.Should().Be(rawPath.TrimEnd('/'));
            path.PathType.Should().Be(PathType.Dir);
        }
        [Fact]
        public void ConvertFile_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"/my/file.txt";
            var path = $@"""{rawPath}""".FromJson<PathInfo>();

            path.RawPath.Should().Be(rawPath.TrimEnd('/'));
            path.PathType.Should().Be(PathType.File);
        }
    }
}
