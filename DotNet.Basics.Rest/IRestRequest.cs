using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public interface IRestRequest
    {
        HttpMethod Method { get; }
        string Uri { get; }
        HttpContent Content { get; }
        HttpRequestHeaders Headers { get; }
        Version Version { get; }

        IRestRequest WithContent(HttpContent content);
        IRestRequest WithJsonContent(string jsonContent);
        IRestRequest WithHeaders(Action<HttpRequestHeaders> headers);
        IRestRequest WithVersion(Version version);

        HttpRequestMessage HttpRequestMessage { get; }
    }
}
