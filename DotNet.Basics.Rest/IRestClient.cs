using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public interface IRestClient : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseAddress { get; set; }
        long MaxResponseContentBufferSize { get; set; }
        TimeSpan Timeout { get; set; }

        Task<HttpResponseMessage> SendAsync(IRestRequest request);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<HttpResponseMessage> DeleteAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> GetAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> HeadAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content = null);
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content = null);
    }
}
