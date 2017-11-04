using System;
using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Rest.Tests
{
    public class RestRequestsTests
    {
        private const string _uri = "http://localhost:8080/my/site?hello=world";

        [Fact]
        public void Delete_CreateRequest_RequestIsCreated()
        {
            var req = Delete.Uri(_uri);
            req.ToString().Should().Be($"DELETE {_uri}");
        }
        [Fact]
        public void Get_CreateRequest_RequestIsCreated()
        {
            var req = Get.Uri(_uri);
            req.ToString().Should().Be($"GET {_uri}");
        }
        [Fact]
        public void Head_CreateRequest_RequestIsCreated()
        {
            var req = Head.Uri(_uri);
            req.ToString().Should().Be($"HEAD {_uri}");
        }
        [Fact]
        public void Post_CreateRequest_RequestIsCreated()
        {
            var req = Post.Uri(_uri);
            req.ToString().Should().Be($"POST {_uri}");
        }
        [Fact]
        public void Put_CreateRequest_RequestIsCreated()
        {
            var req = Put.Uri(_uri);
            req.ToString().Should().Be($"PUT {_uri}");
        }
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
            action.ShouldNotThrow();
        }
    }
}
