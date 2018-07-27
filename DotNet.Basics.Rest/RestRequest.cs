using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public class RestRequest : IRestRequest
    {
        public RestRequest(string uri)
            : this(HttpMethod.Get, uri)
        { }

        public RestRequest(HttpMethod method, string uri)
        {
            HttpRequestMessage = new HttpRequestMessage(method, uri);
        }

        public HttpMethod Method => HttpRequestMessage.Method;
        public string Uri => HttpRequestMessage.RequestUri.AbsoluteUri;
        public HttpContent Content => HttpRequestMessage.Content;
        public HttpRequestHeaders Headers => HttpRequestMessage.Headers;
        public Version Version => HttpRequestMessage.Version;

        public IRestRequest WithContent(HttpContent content)
        {
            if (content != null)
                HttpRequestMessage.Content = content;
            return this;
        }

        public IRestRequest WithJsonContent(string content)
        {
            return WithContent(new JsonContent(content));
        }

        public IRestRequest WithHeaders(Action<HttpRequestHeaders> addHeaders)
        {
            addHeaders?.Invoke(Headers);
            return this;
        }

        public IRestRequest WithVersion(Version version)
        {
            if (Version != null)
                HttpRequestMessage.Version = version;
            return this;
        }

        public HttpRequestMessage HttpRequestMessage { get; }

        public override string ToString()
        {
            return $"{Method} {Uri}";
        }
    }
}
