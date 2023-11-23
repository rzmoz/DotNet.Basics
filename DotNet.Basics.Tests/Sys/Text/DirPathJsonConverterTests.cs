using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class DirPathJsonConverterTests
    {
        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var rawPath = @"c:\my\path\";
            var dir = rawPath.ToDir();

            var json = dir.ToJson();

            json.Should().Be($@"""{rawPath.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"c:\my\path\";
            var path = $@"""{rawPath.Replace("\\", "\\\\")}""".FromJson<DirPath>();

            path.RawPath.Should().Be(rawPath);
        }
    }
}
