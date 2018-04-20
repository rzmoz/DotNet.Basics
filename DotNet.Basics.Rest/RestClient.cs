using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
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

        public Task<HttpResponseMessage> DeleteAsync(string uri, HttpContent content = null)
        {
            var requestMessage = GetQuickRequest(HttpMethod.Delete, uri, content);
            return SendAsync(requestMessage);
        }
        public Task<HttpResponseMessage> GetAsync(string uri, HttpContent content = null)
        {
            var requestMessage = GetQuickRequest(HttpMethod.Get, uri, content);
            return SendAsync(requestMessage);
        }
        public Task<HttpResponseMessage> HeadAsync(string uri, HttpContent content = null)
        {
            var requestMessage = GetQuickRequest(HttpMethod.Head, uri, content);
            return SendAsync(requestMessage);
        }
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content = null)
        {
            var requestMessage = GetQuickRequest(HttpMethod.Post, uri, content);
            return SendAsync(requestMessage);
        }
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content = null)
        {
            var requestMessage = GetQuickRequest(HttpMethod.Put, uri, content);
            return SendAsync(requestMessage);
        }

        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _client.SendAsync(request);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private HttpRequestMessage GetQuickRequest(HttpMethod method, string uri, HttpContent content = null)
        {
            var fullUri = BaseAddress == null ? new Uri(uri) : new Uri(BaseAddress, uri);
            var requestMessage = new HttpRequestMessage(method, fullUri);
            if (content != null)
                requestMessage.Content = content;
            return requestMessage;
        }
    }
}