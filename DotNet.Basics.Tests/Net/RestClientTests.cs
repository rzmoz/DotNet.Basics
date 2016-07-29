using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Net;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Net
{
    [TestFixture]
    public class RestClientTests
    {
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new JsonSerializer();
        }

        [Test]
        public async Task ExecuteTAsync_ContentType_ContentTypeIsAddedToContent()
        {
            var request = new RestRequest("http://dr.dk/", HttpMethod.Post)
            {
                Content = new StringContent("something", Encoding.UTF8, "my/content")
            };
            var client = new DotNet.Basics.Net.RestClient();

            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            ((RestResponse<string>)response).HttpResponseMessage.RequestMessage.Content.Headers.First().Value.First().Should().Be("my/content; charset=utf-8");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ExecuteTAsync_ResponseBodyNotProperValueTo_ExceptionIsThrown()
        {
            var request = new RestRequest("http://dr.dk/", HttpMethod.Get);
            var client = new DotNet.Basics.Net.RestClient();

            Func<Task> action = async () => await client.ExecuteAsync<int>(request).ConfigureAwait(false);

            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task ExecuteTAsync_ValidRquest_RequestIsRecieved()
        {
            var request = new RestRequest("http://dr.dk/", HttpMethod.Get);
            var client = new DotNet.Basics.Net.RestClient();

            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ExecuteAsync_ValidRquest_RequestIsRecieved()
        {
            var request = new RestRequest("http://dr.dk/", HttpMethod.Get);
            var client = new DotNet.Basics.Net.RestClient();

            var response = await client.ExecuteAsync<string>(request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        /*
        [Test]
        public void ExecuteAsync_FailedRequest_NoExceptions()
        {
            const string uri = "http://this.domain.does.not.exist/Something";

            var request = new RestRequest(uri);

            IRestClient client = new DotNet.Basics.Net.RestClient();

            System.Action action = () => { var result = client.ExecuteAsync<string>(request).Result; };

            action.ShouldThrow<RestRequestException>().WithInnerException<HttpRequestException>().Which.Request.Uri.ToString().Should().Be(uri);
        }*/

        [Test]
        public async Task ExecuteAsync_ResponseGottenAndQuotesStripped_ShouldDeserializeString()
        {
            //arrange
            const string connectionString = "\"some connection string\"";

            var httpTransport = Substitute.For<IHttpTransport>();

            httpTransport.SendRequestAsync(Arg.Any<IRestRequest>()).Returns(GetHttpResponseMessageTask(connectionString));
            var restClient = new DotNet.Basics.Net.RestClient(httpTransport);
            var request = new RestRequest("http://myserver.com/string");
            //act 
            var restResponse = await restClient.ExecuteAsync<string>(request, ResponseFormatting.TrimQuotesWhenContentIsString).ConfigureAwait(false);
            //assert
            restResponse.Content.Should().Be(connectionString.Trim('\"'));
        }

        [Test]
        public async Task ExecuteAsync_EmptyResponse_ShouldNotThrowException()
        {
            //arrange
            var httpTransport = Substitute.For<IHttpTransport>();

            httpTransport.SendRequestAsync(Arg.Any<IRestRequest>()).Returns(GetHttpResponseMessageTask(string.Empty));
            var restClient = new DotNet.Basics.Net.RestClient(httpTransport);
            var request = new RestRequest("http://myserver.com/string");
            //act 
            var restResponse = await restClient.ExecuteAsync<string>(request).ConfigureAwait(false);
            //assert
            restResponse.Content.Should().Be(string.Empty);
        }

        [Test]
        public async Task ExecuteAsync_ResponseGotten_ShouldBeDeserialized()
        {
            //arrange
            var client = new TestClient()
            {
                ClientName = "Martin F",
                PhoneNumbers = { { "Home", "+38095234223" }, { "Work", "+380769508682" } }
            };

            var httpTransport = Substitute.For<IHttpTransport>();
            var serializedClient = _serializer.Serialize(client);
            httpTransport.SendRequestAsync(Arg.Any<IRestRequest>()).Returns(GetHttpResponseMessageTask(serializedClient));
            var restClient = new DotNet.Basics.Net.RestClient(httpTransport);
            var request = new RestRequest("http://myserver.com/clients/1");
            //act 
            var restResponse = await restClient.ExecuteAsync<TestClient>(request).ConfigureAwait(false);
            var clientFromRequest = restResponse.Content;
            //assert
            clientFromRequest.ClientName.Should().Be("Martin F");
            clientFromRequest.PhoneNumbers.Should().ContainKey("Home").WhichValue.Should().Be("+38095234223");
            clientFromRequest.PhoneNumbers.Should().ContainKey("Work").WhichValue.Should().Be("+380769508682");
        }

        [Test]
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
            var jsonRestClient = new DotNet.Basics.Net.RestClient(httpTransport);
            //act
            System.Action action = () => { var result = jsonRestClient.ExecuteAsync(restRequest).Result; };
            //assert
            action.ShouldThrow<RestRequestException>().WithInnerException<WebException>().Which.Request.Uri.ToString().Should().Be(uri);
        }
        private Task<HttpResponseMessage> GetHttpResponseMessageTask(string content)
        {
            return Task.Factory.StartNew(() => new HttpResponseMessage { Content = new StringContent(content) });
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
