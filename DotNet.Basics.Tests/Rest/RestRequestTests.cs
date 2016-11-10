using System.Net.Http;
using System.Text;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    
    internal class RestRequestTests
    {
        [Theory]
        [InlineData("http://localhost/", "/path")]//both hostUrl and path has slash
        [InlineData("http://localhost", "path")]//neither hostUrl nor path has slash
        [InlineData("http://localhost/", "path")]//only hostUrl has slash
        [InlineData("http://localhost", "/path")]//only path has slash
        public void Ctor_UrlConcatanation_UrlIsConcatanedCorrectly(string hostUrl, string path)
        {
            var request = new RestRequest(hostUrl, path);

            request.Uri.ToString().Should().Be("http://localhost/path");
        }

        [Fact]
        public void ToString_Formatting_StringIsDebugFriendly()
        {
            var request = new RestRequest("http://dr.dk/mypath", HttpMethod.Put);

            var result = request.ToString();

            result.Should().StartWith("Method: PUT, RequestUri: 'http://dr.dk/mypath', Version: 1.1");
        }

        [Fact]
        public void ToString_FormattingWithcontent_StringIsDebugFriendly()
        {
            var request = new RestRequest("http://dr.dk/mypath", HttpMethod.Post);
            request.Headers.Add("MyHeader1", "HeaderValue1");
            request.Headers.Add("MyHeader2", "HeaderValue2");
            request.Headers.Add("MyHeader3", "HeaderValue3");
            request.Content = new StringContent("MyContent", Encoding.UTF8, "application/json");

            var result = request.ToString();

            result.Should().StartWith("Method: POST, RequestUri: 'http://dr.dk/mypath', Version: 1.1, Content: System.Net.Http.StringContent, Headers:");
            result.Should().Contain("MyHeader1: HeaderValue1");
            result.Should().Contain("MyHeader2: HeaderValue2");
            result.Should().Contain("MyHeader3: HeaderValue3");
        }

        [Fact]
        public void Uri_Get_IsSet()
        {
            const string domain = "www.google.com";

            var request = new RestRequest("http", domain, string.Empty, HttpMethod.Get);

            request.Uri.ToString().Should().Be("http://" + domain + "/");
        }

        [Fact]
        public void Ctor_NoProtocol_Ok()
        {
            var request = new RestRequest("localhost");

            request.Uri.ToString().Should().Be("localhost");
        }

        [Fact]
        public void Ctor_ValidHttpHostUrl_Ok()
        {
            var request = new RestRequest("http://localhost");
            request.Uri.Scheme.Should().Be("http");
            request.Uri.AbsoluteUri.Should().Be("http://localhost/");
            request.Uri.Host.Should().Be("localhost");
        }

        [Fact]
        public void Ctor_ValidHttpHostUrlEndsWithSlash_Ok()
        {
            var request = new RestRequest("http://localhost/");
            request.Uri.Scheme.Should().Be("http");
            request.Uri.AbsoluteUri.Should().Be("http://localhost/");
            request.Uri.Host.Should().Be("localhost");
        }

        [Fact]
        public void Ctor_ValidHttspHostUrl_Ok()
        {
            var request = new RestRequest("https://localhost");
            request.Uri.Scheme.Should().Be("https");
            request.Uri.AbsoluteUri.Should().Be("https://localhost/");
            request.Uri.Host.Should().Be("localhost");
        }

        [Fact]
        public void Ctor_ValidHttspHostUrlEndsWithSlash_Ok()
        {
            var request = new RestRequest("https://localhost/");
            request.Uri.Scheme.Should().Be("https");
            request.Uri.AbsoluteUri.Should().Be("https://localhost/");
            request.Uri.Host.Should().Be("localhost");
        }
    }
}
