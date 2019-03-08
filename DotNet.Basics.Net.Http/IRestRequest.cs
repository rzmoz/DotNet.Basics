using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

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
        IRestRequest WithFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content);

        IRestRequest WithHttpContent(HttpContent content);
        IRestRequest WithHeader(string name, string value);
        IRestRequest WithHeaders(Action<HttpRequestHeaders> headers);
        IRestRequest WithVersion(Version version);

        HttpRequestMessage HttpRequestMessage { get; }
    }
}
