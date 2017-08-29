using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public class RestClient : IRestClient
    {
        private readonly IHttpTransport _transport;
        private readonly Action<string> _debugOut = (msg) => { };//defaults to void

        public RestClient()
            : this(new HttpClientTransport())
        {
        }

        public RestClient(string baseUri)
            : this(new HttpClientTransport
            {
                BaseUri = new Uri(baseUri)
            })
        {
        }

        public RestClient(IHttpTransport transport)
        {
            _transport = transport;
        }

        public HttpRequestHeaders DefaultRequestHeaders => _transport.DefaultRequestHeaders;

        public Uri BaseUri
        {
            get => _transport.BaseUri;
            set => _transport.BaseUri = value;
        }

        public TimeSpan Timeout
        {
            get => _transport.Timeout;
            set => _transport.Timeout = value;
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            HttpResponseMessage response = null;

            try
            {
                _debugOut($"Executing Rest request: {request}");
                response = await _transport.SendRequestAsync(request).ConfigureAwait(false);
                _debugOut($"Rest response received: {response}");
            }
            catch (Exception e)
            {
                _debugOut($"Rest request {request.Uri} failed: {e}");
                throw new RestRequestException(request.Uri.ToString() + " failed", e, request, response);
            }
            return new RestResponse<T>(request.Uri, response, responseFormatting);
        }

        public async Task<IRestResponse> ExecuteAsync(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw)
        {
            return await ExecuteAsync<string>(request, responseFormatting);
        }
    }
}
