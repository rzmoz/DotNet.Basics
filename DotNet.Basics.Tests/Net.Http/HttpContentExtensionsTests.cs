using System;
using System.Threading.Tasks;
using DotNet.Basics.Net.Http;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Net.Http
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


    }
}
