using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DotNet.Basics.Net.Http
{
    public interface IRestRequest
    {
        HttpMethod Method { get; }
        string Uri { get; }
        HttpContent Content { get; }
        HttpRequestHeaders Headers { get; }
        Version Version { get; }

        IRestRequest WithJsonContent(string jsonContent);

        IRestRequest WithHttpContent(HttpContent content);
        IRestRequest WithHeaders(Action<HttpRequestHeaders> headers);
        IRestRequest WithVersion(Version version);

        HttpRequestMessage HttpRequestMessage { get; }
    }
}
