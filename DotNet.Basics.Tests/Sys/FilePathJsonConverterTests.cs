using System.Text.Json;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class FilePathJsonConverterTests
    {
        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var rawPath = @"c:\my\file.txt";
            var file = rawPath.ToFile();

            var json = JsonSerializer.Serialize(file, new JsonSerializerOptions
            {
                Converters = { new FilePathJsonConverter() }
            });

            json.Should().Be($@"""{rawPath.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"c:\my\file.txt";
            var path = JsonSerializer.Deserialize<FilePath>($@"""{rawPath.Replace("\\", "\\\\")}""", new JsonSerializerOptions
            {
                Converters = { new FilePathJsonConverter() }
            });

            path.RawPath.Should().Be(rawPath);
        }
    }
}
