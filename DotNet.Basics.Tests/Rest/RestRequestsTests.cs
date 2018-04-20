using System;
using System.Net.Http;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class RestRequestsTests
    {
        private const string _uri = "http://localhost:8080/my/site?hello=world";

        [Fact]
        public void Custom_CreateRequest_RequestIsCreated()
        {
            const string method = "Baflstwets";
            var req = new RestRequest(new HttpMethod(method), _uri);
            req.ToString().Should().Be($"{method } {_uri}");
        }

        [Theory]
        [InlineData("/localhost/")]//no scheme
        [InlineData("http:/localhost/")]//invalid scheme separator
        public void Ctor_NotUri_ExceptionIsThrown(string nonUri)
        {
            Action action = () => new RestRequest(nonUri);
            action.Should().NotThrow();
        }
    }
}
