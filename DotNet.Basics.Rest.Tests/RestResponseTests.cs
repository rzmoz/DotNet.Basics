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
    public class RestResponseTests
    {
        [Fact]
        public async Task Headers_HeadersInResponse_Captured()
        {
            var request = new RestRequest("https://www.google.com/", HttpMethod.Get);
            
            IRestClient client = new RestClient();

            var response = await client.ExecuteAsync<string>(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.HttpResponseMessage.Headers.GetValues("Cache-Control").Single().Should().Be("max-age=0, private");
            response.HttpResponseMessage.Headers.GetValues("Server").Single().Should().Be("gws");
            response.HttpResponseMessage.Headers.Count().Should().BeGreaterOrEqualTo(11);
        }

        [Fact]
        public async Task ToString_Debug_StringIsDebugFriendly()
        {
            var request = new RestRequest("http://dr.dk/");


            IRestClient client = new RestClient();

            var response = await client.ExecuteAsync<string>(request);
            var result = response.ToString();

            result.Should().StartWith("StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.NoWriteNoSeekStreamContent, Headers:");
        }

        [Fact]
        public void Content_TrimContent_ContentIsTrimmed()
        {
            var responseMessage = new HttpResponseMessage { Content = new StringContent(@"""Content With Quotes""") };
            var response = new RestResponse(new Uri("http://localhost"), responseMessage, ResponseFormatting.TrimQuotesWhenContentIsString);

            response.Body.Should().Be("Content With Quotes");//but without quotes..
        }

        [Fact]
        public void Content_TrimContent_ContentIsNotTrimmed()
        {
            var contentWithQuotes = @"""Content With Quotes""";
            var responseMessage = new HttpResponseMessage { Content = new StringContent(contentWithQuotes) };
            var response = new RestResponse(new Uri("http://localhost"), responseMessage, ResponseFormatting.Raw);

            response.Body.Should().Be(contentWithQuotes);
        }
    }
}
