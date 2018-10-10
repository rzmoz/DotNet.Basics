using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Net.Http;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNet.Basics.Tests.Net.Http
{
    public class RestClientTests
    {
        private readonly IRestClient _echoRestClient;

        public RestClientTests()
        {
            var echoHttpClient = Substitute.For<HttpClient>();
            echoHttpClient.SendAsync(Arg.Any<HttpRequestMessage>(), CancellationToken.None)
                .ReturnsForAnyArgs(info => new HttpResponseMessage { RequestMessage = info.Arg<HttpRequestMessage>() });

            _echoRestClient = new RestClient("http://localhost/", echoHttpClient);
        }

        [Fact]
        public async Task DeleteAsync_Invoke_MethodIsSet()
        {
            var response = await _echoRestClient.DeleteAsync("").ConfigureAwait(false);
            response.RequestMessage.Method.Should().Be(HttpMethod.Delete);
        }
        [Fact]
        public async Task GetAsync_Invoke_MethodIsSet()
        {
            var response = await _echoRestClient.GetAsync("").ConfigureAwait(false);
            response.RequestMessage.Method.Should().Be(HttpMethod.Get);
        }
        [Fact]
        public async Task HeadAsync_Invoke_MethodIsSet()
        {
            var response = await _echoRestClient.HeadAsync("").ConfigureAwait(false);
            response.RequestMessage.Method.Should().Be(HttpMethod.Head);
        }
        [Fact]
        public async Task PostAsync_Invoke_MethodIsSet()
        {
            var response = await _echoRestClient.PostAsync("").ConfigureAwait(false);
            response.RequestMessage.Method.Should().Be(HttpMethod.Post);
        }
        [Fact]
        public async Task PutAsync_Invoke_MethodIsSet()
        {
            var response = await _echoRestClient.PutAsync("").ConfigureAwait(false);
            response.RequestMessage.Method.Should().Be(HttpMethod.Put);
        }


        [Fact]
        public async Task DefaultRequestHeaderes_DefaultHeaders_HeadersAreSet()
        {
            var headerKey = "X-DotNet.Basics";
            var headerValue = "yEs";

            IRestClient client = new RestClient("https://code.jquery.com/");
            client.DefaultRequestHeaders.Add(headerKey, headerValue);

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            var requestHeaders = response.RequestMessage.Headers.GetValues(headerKey).ToList();
            requestHeaders.Count.Should().Be(1);
            requestHeaders.Single().Should().Be(headerValue);
        }

        [Fact]
        public async Task BaseUri_BaseUriIsSet_UriIsProper()
        {
            IRestClient client = new RestClient("https://code.jquery.com/");

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.RequestMessage.RequestUri.ToString().Should().Be("https://code.jquery.com/jquery-1.12.4.min.js");
        }

        [Fact]
        public async Task SendAsync_ValidRequest_ResponseIsReceived()
        {
            IRestClient client = new RestClient("https://code.jquery.com/");

            var response = await client.GetAsync("jquery-1.12.4.min.js").ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SendAsync_IRequest_ResponseIsReceived()
        {
            IRestClient client = new RestClient("https://code.jquery.com/");
            IRestRequest request = new RestRequest(HttpMethod.Get, "jquery-1.12.4.min.js");

            var response = await client.SendAsync(request).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RequestSending_Events_EventIsRaised()
        {
            //arrange
            IRestClient client = new RestClient("https://cdnjs.cloudflare.com/ajax/libs/mogl/0.3.0/");

            HttpRequestMessage receivedRequest = null;
            HttpResponseMessage receivedResponse = null;

            client.RequestSending += req => receivedRequest = req;
            client.ResponseReceived += res => receivedResponse = res;

            //act
            var response = await client.GetAsync("mogl.min.js").ConfigureAwait(false);

            //assert
            receivedRequest.Should().NotBeNull();
            receivedResponse.Should().NotBeNull();
            receivedRequest.RequestUri.ToString().Should().Be("https://cdnjs.cloudflare.com/ajax/libs/mogl/0.3.0/mogl.min.js");
            receivedResponse.RequestMessage.RequestUri.ToString().Should().Be("https://cdnjs.cloudflare.com/ajax/libs/mogl/0.3.0/mogl.min.js");
        }
    }
}
