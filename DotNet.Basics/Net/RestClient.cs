using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Basics.Net
{
    public class RestClient : IRestClient
    {
        private readonly IHttpTransport _transport;

        internal RestClient(IHttpTransport transport)
        {
            _transport = transport;
        }

        public RestClient()
            : this(new HttpClientTransport())
        {
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            HttpResponseMessage response = null;

            try
            {
                response = await _transport.SendRequestAsync(request).ConfigureAwait(false);
            }
            catch (Exception e)
            {
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
