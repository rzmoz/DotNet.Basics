using System.Text.Json;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class DirPathJsonConverterTests
    {
        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var rawPath = @"c:\my\path\";
            var dir = rawPath.ToDir();

            var json = JsonSerializer.Serialize(dir, new JsonSerializerOptions
            {
                Converters = { new DirPathJsonConverter() }
            });

            json.Should().Be($@"""{rawPath.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"c:\my\path\";
            var path = JsonSerializer.Deserialize<DirPath>($@"""{rawPath.Replace("\\", "\\\\")}""", new JsonSerializerOptions
            {
                Converters = { new DirPathJsonConverter() }
            });

            path.RawPath.Should().Be(rawPath);
        }
    }
}
