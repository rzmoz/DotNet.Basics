using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public class HttpClientTransport : IHttpTransport
    {
        public async Task<HttpResponseMessage> SendRequestAsync(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            //we default to 1 hour timeout
            var httpClient = new HttpClient()
            {
                Timeout = request.TimeOut
            };

            return await httpClient.SendAsync(request.HttpRequestMessage).ConfigureAwait(false);
        }
    }
}