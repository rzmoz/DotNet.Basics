using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Exceptions;
using Xunit;

namespace DotNet.Basics.Rest.Tests
{
    public class RestClientTests
    {
        [Fact]
        public async Task DefaultRequestHeaderes_DefaultHeaders_HeadersAreSet()
        {
            var headerKey = "X-DotNet.Basics";
            var headerValue = "yEs";

            var client = new RestClient("https://code.jquery.com/");
            client.DefaultRequestHeaders.Add(headerKey, headerValue);

            var response = await Get.Uri("jquery-1.12.4.min.js").SendAsync(client).ConfigureAwait(false);

            var requestHeaders = response.RequestMessage.Headers.GetValues(headerKey).ToList();
            requestHeaders.Count.Should().Be(1);
            requestHeaders.Single().Should().Be(headerValue);
        }

        [Fact]
        public async Task BaseUri_BaseUriIsSet_UriIsProper()
        {
            var client = new RestClient("https://code.jquery.com/");

            var response = await Get.Uri("jquery-1.12.4.min.js").SendAsync(client).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.RequestMessage.RequestUri.ToString().Should().Be("https://code.jquery.com/jquery-1.12.4.min.js");
        }

        [Fact]
        public async Task ExecuteAsync_ValidRquest_RequestIsReceived()
        {
            var client = new RestClient("https://code.jquery.com/");

            var response = await Get.Uri("jquery-1.12.4.min.js").SendAsync(client).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ExecuteAsync_FailedRequest_NoExceptions()
        {
            var uri = "http://this.domain.does.not.exist/Something";

            Func<Task> act = async () =>
            {
                await Get.Uri(uri).SendAsync(new RestClient()).ConfigureAwait(false);
            };

            act.ShouldThrow<HttpRequestException>();
        }
    }
}
