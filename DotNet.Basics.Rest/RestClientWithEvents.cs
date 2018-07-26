using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public class RestClientWithEvents : RestClient
    {
        public delegate void RequestHandler(HttpRequestMessage request);
        public delegate void ResponseHandler(HttpResponseMessage response);

        public event RequestHandler RequestSending;
        public event ResponseHandler ResponseReceived;

        public RestClientWithEvents(string baseUri = null, HttpClient httpClient = null) : base(baseUri, httpClient)
        { }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            RequestSending?.Invoke(request);
            var response = await base.SendAsync(request).ConfigureAwait(false);
            ResponseReceived?.Invoke(response);
            return response;
        }
    }
}
