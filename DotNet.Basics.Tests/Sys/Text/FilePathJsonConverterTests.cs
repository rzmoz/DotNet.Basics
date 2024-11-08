using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class FilePathJsonConverterTests
    {
        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var rawPath = @"/my/file.txt";
            var file = rawPath.ToFile();

            var json = file.ToJson();

            json.Should().Be($@"""{rawPath}""");
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"/my/file.txt";
            var path = $@"""{rawPath}""".FromJson<FilePath>();

            path.RawPath.Should().Be(rawPath);
        }
    }
}
