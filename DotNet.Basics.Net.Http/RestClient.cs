using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Net.Http
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _client;

        public RestClient(string baseUri = null, IEnumerable<KeyValuePair<string, string>> defaultRequestHeaders = null, HttpClient httpClient = null)
        {
            _client = httpClient ?? new HttpClient();
            if (baseUri != null)
                _client.BaseAddress = new Uri(baseUri);

            if (defaultRequestHeaders != null)
                foreach (var header in defaultRequestHeaders)
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);

            _client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
        }

        public delegate void RequestHandler(HttpRequestMessage request);
        public delegate void ResponseHandler(HttpResponseMessage response);

        public event RequestHandler RequestSending;
        public event ResponseHandler ResponseReceived;

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
            return SendAsync(HttpMethod.Delete, uri, content);
        }
        public Task<HttpResponseMessage> GetAsync(string uri, HttpContent content = null)
        {
            return SendAsync(HttpMethod.Get, uri, content);
        }
        public Task<HttpResponseMessage> HeadAsync(string uri, HttpContent content = null)
        {
            return SendAsync(HttpMethod.Head, uri, content);
        }
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content = null)
        {
            return SendAsync(HttpMethod.Post, uri, content);
        }
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content = null)
        {
            return SendAsync(HttpMethod.Put, uri, content);
        }

        private Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, HttpContent content = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            if (content != null)
                requestMessage.Content = content;
            return SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> SendAsync(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return SendAsync(request.HttpRequestMessage);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            RequestSending?.Invoke(request);
            var response = await _client.SendAsync(request, CancellationToken.None).ConfigureAwait(false);
            ResponseReceived?.Invoke(response);
            return response;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}