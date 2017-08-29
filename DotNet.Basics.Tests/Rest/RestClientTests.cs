using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Rest;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class RestClientTests
    {
        [Fact]
        public void Timeout_PropagatedToTransportLayer_TimeoutIsSet()
        {
            var customTimeout = TimeSpan.FromTicks(143423);
            var transport = new HttpClientTransport();
            transport.HttpClient.Timeout.Should().NotBe(customTimeout);

            var client = new RestClient(transport)
            {
                Timeout = customTimeout
            };

            client.Timeout.Should().Be(transport.HttpClient.Timeout);
            transport.HttpClient.Timeout.Should().Be(customTimeout);
        }
        
        [Fact]
        public async Task DefaultRequestHeaderes_DefaultHeaders_HeadersAreSet()
        {
            var headerKey = "X-DotNet.Basics";
            var headerValue = "yEs";

            var client = new RestClient("https://code.jquery.com/");
            client.DefaultRequestHeaders.Add(headerKey, headerValue);

            var request = new RestRequest("jquery-1.12.4.min.js", HttpMethod.Get);
            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            var requestHeaders = response.HttpResponseMessage.RequestMessage.Headers.GetValues(headerKey).ToList();
            requestHeaders.Count.Should().Be(1);
            requestHeaders.Single().Should().Be(headerValue);
        }

        [Fact]
        public async Task BaseUri_BaseUriIsSet_UriIsProper()
        {
            var baseUri = "https://code.jquery.com/";

            var client = new RestClient(baseUri);

            var request = new RestRequest("jquery-1.12.4.min.js", HttpMethod.Get);
            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task ExecuteTAsync_ValidRquest_RequestIsReceived()
        {
            var request = new RestRequest("https://code.jquery.com/jquery-1.12.4.min.js", HttpMethod.Get);
            var client = new RestClient();

            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExecuteAsync_ValidRquest_RequestIsReceived()
        {
            var request = new RestRequest("https://code.jquery.com/jquery-1.12.4.min.js", HttpMethod.Get);
            var client = new RestClient();

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /*
        [Fact]
        public void ExecuteAsync_FailedRequest_NoExceptions()
        {
            const string uri = "http://this.domain.does.not.exist/Something";

            var request = new RestRequest(uri);

            IRestClient client = new RestClient();

            System.Action action = () => { var result = client.ExecuteAsync<string>(request).Result; };

            action.ShouldThrow<RestRequestException>().WithInnerException<HttpRequestException>().Which.Request.Uri.ToString().Should().Be(uri);
        }*/

        [Fact]
        public void ExecuteTAsync_ContentType_ContentTypeIsAddedToContent()
        {
            var request = new RestRequest("https://files-stackablejs.netdna-ssl.com/stacktable.min.js/", HttpMethod.Post)
            {
                Content = new StringContent("something", Encoding.UTF8, "my/content")
            };

            request.Content.Headers.First().Value.First().Should().Be("my/content; charset=utf-8");
        }


        [Fact]
        public void ExecuteTAsync_ResponseBodyNotProperValueTo_ExceptionIsThrown()
        {
            var request = new RestRequest("https://my.server.com", HttpMethod.Get);
            var client = GetRestClientWithResponseContent("string.not.int");

            Func<Task> action = async () => await client.ExecuteAsync<int>(request).ConfigureAwait(false);

            action.ShouldThrow<RestReaderException>();
        }

        [Fact]
        public async Task ExecuteAsync_ResponseGottenAndQuotesStripped_ShouldDeserializeString()
        {
            //arrange
            const string contentWithQuotes = "\"some connection string\"";
            var restClient = GetRestClientWithResponseContent(contentWithQuotes);
            var request = new RestRequest("http://myserver.com/string");
            //act 
            var restResponse = await restClient.ExecuteAsync<string>(request, ResponseFormatting.TrimQuotesWhenContentIsString).ConfigureAwait(false);
            //assert
            restResponse.Content.Should().Be(contentWithQuotes.Trim('\"'));
        }

        [Fact]
        public async Task ExecuteAsync_EmptyResponse_ShouldNotThrowException()
        {
            //arrange
            var restClient = GetRestClientWithResponseContent(string.Empty);
            var request = new RestRequest("http://myserver.com/string");
            //act 
            var restResponse = await restClient.ExecuteAsync<string>(request).ConfigureAwait(false);
            //assert
            restResponse.Content.Should().Be(string.Empty);
        }

        [Fact]
        public async Task ExecuteAsync_ResponseGotten_ShouldBeDeserialized()
        {
            //arrange
            var serializedClient = JsonConvert.SerializeObject(new TestClient()
            {
                ClientName = "Martin F",
                PhoneNumbers = { { "Home", "+38095234223" }, { "Work", "+380769508682" } }
            });

            var restClient = GetRestClientWithResponseContent(serializedClient);
            var request = new RestRequest("http://myserver.com/clients/1");
            //act 
            var restResponse = await restClient.ExecuteAsync<TestClient>(request).ConfigureAwait(false);
            var clientFromRequest = restResponse.Content;
            //assert
            clientFromRequest.ClientName.Should().Be("Martin F");
            clientFromRequest.PhoneNumbers.Should().ContainKey("Home").WhichValue.Should().Be("+38095234223");
            clientFromRequest.PhoneNumbers.Should().ContainKey("Work").WhichValue.Should().Be("+380769508682");
        }

        [Fact]
        public void ExecuteAsync_ExceptionThrown_ShoulBeHandled()
        {
            //arrange
            var httpTransport = Substitute.For<IHttpTransport>();
            const string uri = "http://myserver.com/shouldthrow";
            httpTransport.When(t => t.SendRequestAsync(Arg.Any<IRestRequest>())).Do(x =>
            {
                throw new WebException("Exception thrown");
            });
            var restRequest = new RestRequest(uri);
            var jsonRestClient = new RestClient(httpTransport);
            //act
            Action action = () => { var result = jsonRestClient.ExecuteAsync(restRequest).Result; };
            //assert
            action.ShouldThrow<RestRequestException>().WithInnerException<WebException>().Which.Request.Uri.ToString().Should().Be(uri);
        }

        private IRestClient GetRestClientWithResponseContent(string content)
        {
            var httpTransport = Substitute.For<IHttpTransport>();
            httpTransport.SendRequestAsync(Arg.Any<IRestRequest>()).Returns(GetHttpResponseMessageTask(content));
            return new RestClient(httpTransport);
        }

        private Task<HttpResponseMessage> GetHttpResponseMessageTask(string content)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                Content = new StringContent(content)
            });
        }
    }

    [DataContract]
    public class TestClient
    {
        public TestClient()
        {
            PhoneNumbers = new Dictionary<string, string>();
        }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public Dictionary<string, string> PhoneNumbers { get; set; }
    }
}
