using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Net.Http
{
    public interface IRestClient : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseAddress { get; set; }
        long MaxResponseContentBufferSize { get; set; }
        TimeSpan Timeout { get; set; }

        event RestClient.RequestHandler RequestSending;
        event RestClient.ResponseHandler ResponseReceived;

        Task<HttpResponseMessage> SendAsync(IRestRequest request);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<HttpResponseMessage> DeleteAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> GetAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> HeadAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content = null);
    }
}
