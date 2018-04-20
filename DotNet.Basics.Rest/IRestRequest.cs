using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public interface IRestRequest
    {
        IRestRequest WithContent(HttpContent content);
        IRestRequest WithJsonContent(string jsonContent);
        IRestRequest WithHeaders(Action<HttpRequestHeaders> headers);
        IRestRequest WithVersion(Version version);
    }
}
