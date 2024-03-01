using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class SemVersionJsonConverterTests
    {
        private const string _rawJson = @"""1.0.3-beta.1.2\u002BHelloWorld""";
        private const string _rawVersion = @"1.0.3-beta.1.2+HelloWorld";

        [Fact]
        public void Convert_Serialize_SemVerIsSerialized()
        {
            var semVer = new SemVersion(_rawVersion);

            var json = semVer.ToJson();

            json.Should().Be(_rawJson);
        }

        [Fact]
        public void Convert_FromJson_SemVerIsDeserialized()
        {
            var version = _rawJson.FromJson<SemVersion>();

            version.SemVer20String.Should().Be(_rawVersion);
        }

        [Fact]
        public void FromJsonStream_Semver_StreamIsDeserialized()
        {
            var baseType = typeof(SemVersionJsonConverterTests);
            using var jsonStream = baseType.Assembly.GetManifestResourceStream($"{baseType.Namespace}.SemVerTest.json");
            var version = jsonStream.FromJsonStream<SemVersion>();

            version.SemVer20String.Should().Be(_rawVersion);
        }
    }
}
