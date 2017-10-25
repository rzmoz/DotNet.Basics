using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public interface IHttpTransport
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseUri { get; set; }
        TimeSpan Timeout { get; set; }
        Task<HttpResponseMessage> SendRequestAsync(IRestRequest request);
    }
}