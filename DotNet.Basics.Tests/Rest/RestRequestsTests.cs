using System;
using System.Linq;
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
        public void Delete_CreateRequest_RequestIsCreated()
        {
            var req = new RestRequest(HttpMethod.Delete, _uri);
            req.Method.Should().Be(HttpMethod.Delete);
        }

        [Fact]
        public void Get_CreateRequest_RequestIsCreated()
        {
            var req = new RestRequest(HttpMethod.Get, _uri);
            req.Method.Should().Be(HttpMethod.Get);
        }

        [Fact]
        public void Head_CreateRequest_RequestIsCreated()
        {
            var req = new RestRequest(HttpMethod.Head, _uri);
            req.Method.Should().Be(HttpMethod.Head);
        }

        [Fact]
        public void Post_CreateRequest_RequestIsCreated()
        {
            var req = new RestRequest(HttpMethod.Post, _uri);
            req.Method.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public void Put_CreateRequest_RequestIsCreated()
        {
            var req = new RestRequest(HttpMethod.Put, _uri);
            req.Method.Should().Be(HttpMethod.Put);
        }


        [Fact]
        public void Custom_CreateRequest_RequestIsCreated()
        {
            const string method = "Baflstwets";
            var req = new RestRequest(new HttpMethod(method), _uri);
            req.ToString().Should().Be($"{method } {_uri}");
        }

        [Fact]
        public void Ctor_HttpRequestMessageDefaultValues_ValuesAreDefault()
        {
            var relativeUri = "HelloWorld";
            var httpRequest = new HttpRequestMessage(HttpMethod.Head, relativeUri);
            httpRequest.Method.Should().Be(HttpMethod.Head);
            httpRequest.Version.Should().Be(new Version(1, 1));
            httpRequest.Headers.Should().BeEmpty();
        }


        [Fact]
        public void GetHttpRequestMessage_MinimumValues_ValuesAreUpdatedProperly()
        {
            //arrange
            var method = HttpMethod.Put;
            var request = new RestRequest(method, _uri);
            var headerKey = "MyHeaderKey";
            var headerValue = "sdfgsdrgsdgse4 gw34taw4gfe w4gfaw3rwtr";

            //act
            request.Headers.Add(headerKey, new[] { headerValue });
            var httpRequest = request.HttpRequestMessage;

            //assert
            httpRequest.RequestUri.ToString().Should().Be(_uri);
            httpRequest.Content.Should().BeNull();
            httpRequest.Version.Should().NotBeNull();
            httpRequest.Headers.GetValues(headerKey).Single().Should().Be(headerValue);
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
