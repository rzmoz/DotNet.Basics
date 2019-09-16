using System.Text.Json;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PathInfoJsonConverterTests
    {
        [Fact]
        public void ConvertDir_Serialize_PathIsSerialized()
        {
            var rawPath = @"c:\my\dirPath\";
            var path = rawPath.ToPath();

            var json = JsonSerializer.Serialize(path, new JsonSerializerOptions
            {
                Converters = { new PathInfoJsonConverter() }
            });

            json.Should().Be($@"""{rawPath.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void ConvertFile_Serialize_PathIsSerialized()
        {
            var rawPath = @"c:\my\file.txt";
            var path = rawPath.ToPath();

            var json = JsonSerializer.Serialize(path, new JsonSerializerOptions
            {
                Converters = { new PathInfoJsonConverter() }
            });

            json.Should().Be($@"""{rawPath.Replace("\\", "\\\\")}""");
        }

        [Fact]
        public void ConvertDir_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"c:\my\path\";
            var path = JsonSerializer.Deserialize<PathInfo>($@"""{rawPath.Replace("\\", "\\\\")}""", new JsonSerializerOptions
            {
                Converters = { new PathInfoJsonConverter() }
            });

            path.RawPath.Should().Be(rawPath);
            path.PathType.Should().Be(PathType.Dir);
        }
        [Fact]
        public void ConvertFile_Deserialize_PathIsDeserialized()
        {
            var rawPath = @"c:\my\file.txt";
            var path = JsonSerializer.Deserialize<PathInfo>($@"""{rawPath.Replace("\\", "\\\\")}""", new JsonSerializerOptions
            {
                Converters = { new PathInfoJsonConverter() }
            });

            path.RawPath.Should().Be(rawPath);
            path.PathType.Should().Be(PathType.File);
        }
    }
}
