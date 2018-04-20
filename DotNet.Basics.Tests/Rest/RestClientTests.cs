using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class RestClientTests
    {
        private const string _uri = "http://localhost:8080/my/site?hello=world";

        [Fact]
        public async Task Delete_CreateRequest_RequestIsCreated()
        {
            var client = new TestRestClient();
            await client.DeleteAsync(_uri).ConfigureAwait(false);
            client.Request.Method.Should().Be(HttpMethod.Delete);
        }

        [Fact]
        public async Task Get_CreateRequest_RequestIsCreated()
        {
            var client = new TestRestClient();
            await client.GetAsync(_uri).ConfigureAwait(false);
            client.Request.Method.Should().Be(HttpMethod.Get);
        }

        [Fact]
        public async Task Head_CreateRequest_RequestIsCreated()
        {
            var client = new TestRestClient();
            await client.HeadAsync(_uri).ConfigureAwait(false);
            client.Request.Method.Should().Be(HttpMethod.Head);
        }

        [Fact]
        public async Task Post_CreateRequest_RequestIsCreated()
        {
            var client = new TestRestClient();
            await client.PostAsync(_uri).ConfigureAwait(false);
            client.Request.Method.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public async Task Put_CreateRequest_RequestIsCreated()
        {
            var client = new TestRestClient();
            await client.PutAsync(_uri).ConfigureAwait(false);
            client.Request.Method.Should().Be(HttpMethod.Put);
        }


        [Fact]
        public async Task DefaultRequestHeaderes_DefaultHeaders_HeadersAreSet()
        {
            var headerKey = "X-DotNet.Basics";
            var headerValue = "yEs";

            var client = new RestClient("https://code.jquery.com/");
            client.DefaultRequestHeaders.Add(headerKey, headerValue);

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            var requestHeaders = response.RequestMessage.Headers.GetValues(headerKey).ToList();
            requestHeaders.Count.Should().Be(1);
            requestHeaders.Single().Should().Be(headerValue);
        }

        [Fact]
        public async Task BaseUri_BaseUriIsSet_UriIsProper()
        {
            var client = new RestClient("https://code.jquery.com/");

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.RequestMessage.RequestUri.ToString().Should().Be("https://code.jquery.com/jquery-1.12.4.min.js");
        }

        [Fact]
        public async Task ExecuteAsync_ValidRquest_RequestIsReceived()
        {
            var client = new RestClient("https://code.jquery.com/");

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        private class TestRestClient : RestClient
        {
            public HttpRequestMessage Request { get; set; }

            public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            {
                Request = request;
                return Task.FromResult(new HttpResponseMessage());
            }
        }
    }


}
