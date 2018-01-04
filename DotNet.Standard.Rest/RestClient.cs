using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Standard.Rest
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _client;

        public RestClient(string baseUri = null, HttpClient httpClient = null)
        {
            _client = httpClient ?? new HttpClient();
            if (baseUri != null)
                _client.BaseAddress = new Uri(baseUri);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            _client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
        }

        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;

        public Uri BaseAddress
        {
            get => _client.BaseAddress;
            set => _client.BaseAddress = value;
        }

        public long MaxResponseContentBufferSize
        {
            get => _client.MaxResponseContentBufferSize;
            set => _client.MaxResponseContentBufferSize = value;

        }

        public TimeSpan Timeout
        {
            get => _client.Timeout;
            set => _client.Timeout = value;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _client.SendAsync(request);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}