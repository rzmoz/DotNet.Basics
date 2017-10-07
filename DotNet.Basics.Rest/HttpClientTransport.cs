using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public class HttpClientTransport : IHttpTransport
    {
        public HttpClient HttpClient { get; }

        public HttpClientTransport()
        {
            HttpClient = new HttpClient();
        }

        public HttpRequestHeaders DefaultRequestHeaders => HttpClient.DefaultRequestHeaders;

        public Uri BaseUri
        {
            get => HttpClient.BaseAddress;
            set => HttpClient.BaseAddress = value;
        }

        public TimeSpan Timeout
        {
            get => HttpClient.Timeout;
            set => HttpClient.Timeout = value;
        }


        public async Task<HttpResponseMessage> SendRequestAsync(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return await HttpClient.SendAsync(request.HttpRequestMessage).ConfigureAwait(false);
        }
    }
}