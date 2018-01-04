using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Standard.Rest
{
    public interface IRestClient : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseAddress { get; set; }
        long MaxResponseContentBufferSize { get; set; }
        TimeSpan Timeout { get; set; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
