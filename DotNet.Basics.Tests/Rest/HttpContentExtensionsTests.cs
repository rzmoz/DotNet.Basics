using System;
using System.Threading.Tasks;
using DotNet.Basics.Rest;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class HttpContentExtensionsTests
    {
        private const string _mutableString = "mutable";
        private const string _immutableString = "immutable";

        private readonly TestObject _testObject = new TestObject(_immutableString)
        {
            Mutable = _mutableString
        };


        [Theory]
        [InlineData(null, null)]//null
        [InlineData("", "")]//empty
        [InlineData("     ", "")]//empty spaces
        [InlineData("\"", "")]//single quote
        [InlineData("\"\"", "")]//double quotes
        [InlineData("\"trimmed\"", "trimmed")]//double quotes
        public void TrimQuotes(string input, string expected)
        {
            var result = HttpContentExtensions.TrimQuotes(input);

            result.Should().Be(expected);
        }

        [Fact]
        public void Ctor_JsonSerializerSettings_SettingsAreUsed()
        {
            //json with superfluous member
            var content = new JsonContent(@"{""Mutable"":""mutable"",""Immutable"":""immutable"",""MissingMember"":""value""}");

            //act
            Func<Task> act = () => content.ReadAsTypeAsync<TestObject>(new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            });

            act.Should().Throw<JsonSerializationException>();
        }


        [Fact]
        public async Task Ctor_Serialization_ContentIsSerilaizedToProperJson()
        {
            //act
            var content = new JsonContent(_testObject);

            var testObjectJson = await content.ReadAsStringAsync().ConfigureAwait(false);

            testObjectJson.Should().Be(@"{""Mutable"":""mutable"",""Immutable"":""immutable""}");
        }


        [Fact]
        public async Task ReadAsType_ToType_TypeIsDeserialized()
        {
            var content = new JsonContent(_testObject);

            var testObject = await content.ReadAsTypeAsync<TestObject>().ConfigureAwait(false);

            testObject.Immutable.Should().Be(_immutableString);
            testObject.Mutable.Should().Be(_mutableString);
        }
    }
}
