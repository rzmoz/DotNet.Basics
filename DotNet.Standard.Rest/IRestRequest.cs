using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Standard.Rest
{
    public interface IRestRequest
    {
        IRestRequest WithContent(HttpContent content);
        IRestRequest WithJsonContent(string jsonContent);
        IRestRequest WithHeaders(Action<HttpRequestHeaders> headers);
        IRestRequest WithVersion(Version version);
        Task<HttpResponseMessage> SendAsync(IRestClient client);
    }
}
