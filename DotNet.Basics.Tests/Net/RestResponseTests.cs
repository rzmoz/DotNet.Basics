using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Basics.Net;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Net
{
    [TestFixture]
    public class RestResponseTests
    {
        [Test]
        public async Task Headers_HeadersInResponse_Captured()
        {
            var request = new RestRequest("http://www.dr.dk", HttpMethod.Get);

            IRestClient client = new DotNet.Basics.Net.RestClient();

            var response = await client.ExecuteAsync<string>(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.HttpResponseMessage.Headers.GetValues("X-Powered-By").Single().Should().Be("ASP.NET");
            response.HttpResponseMessage.Headers.Count().Should().Be(16);
        }

        [Test]
        public async Task ToString_Debug_StringIsDebugFriendly()
        {
            var request = new RestRequest("http://dr.dk/");


            IRestClient client = new DotNet.Basics.Net.RestClient();

            var response = await client.ExecuteAsync<string>(request);
            var result = response.ToString();

            result.Should().StartWith("StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:");
        }

        [Test]
        public void Content_TrimContent_ContentIsTrimmed()
        {
            var responseMessage = new HttpResponseMessage { Content = new StringContent(@"""Content With Quotes""") };
            var response = new RestResponse<string>(new Uri("http://localhost"), responseMessage, ResponseFormatting.TrimQuotesWhenContentIsString);

            response.Content.Should().Be("Content With Quotes");//but without quotes..
        }

        [Test]
        public void Content_TrimContent_ContentIsNotTrimmed()
        {
            var contentWithQuotes = @"""Content With Quotes""";
            var responseMessage = new HttpResponseMessage { Content = new StringContent(contentWithQuotes) };
            var response = new RestResponse<string>(new Uri("http://localhost"), responseMessage, ResponseFormatting.Raw);

            response.Content.Should().Be(contentWithQuotes);
        }
    }
}
