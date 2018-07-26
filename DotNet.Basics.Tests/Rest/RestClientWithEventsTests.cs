using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class RestClientWithEventsTests
    {
        [Fact]
        public async Task RequestSending_Events_EventIsRaised()
        {
            //arrange
            var client = new RestClientWithEvents("https://cdnjs.cloudflare.com/ajax/libs/mogl/0.3.0/");

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
