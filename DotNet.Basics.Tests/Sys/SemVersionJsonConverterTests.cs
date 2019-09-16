using System.Text.Json;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class SemVersionJsonConverterTests
    {
        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var rawVersion = @"1.0.3-beta.1.2+HelloWorld";
            var semVer = new SemVersion(rawVersion);

            var json = JsonSerializer.Serialize(semVer, new JsonSerializerOptions
            {
                Converters = { new SemVersionJsonConverter() }
            });

            json.Should().Be($@"""{rawVersion.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var rawVersion = @"1.0.3-beta.1.2+HelloWorld";
            var version = JsonSerializer.Deserialize<SemVersion>($@"""{rawVersion.Replace("\\", "\\\\")}""", new JsonSerializerOptions
            {
                Converters = { new SemVersionJsonConverter() }
            });

            version.SemVer20String.Should().Be(rawVersion);
        }
    }
}
