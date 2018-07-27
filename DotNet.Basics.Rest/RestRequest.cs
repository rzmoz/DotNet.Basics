using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public class RestRequest : IRestRequest
    {
        public RestRequest(string uri)
            : this(HttpMethod.Get, uri)
        { }

        public RestRequest(HttpMethod method, string uri, HttpContent content = null)
        {
            Method = method;
            Uri = uri;
            Content = content;
            AddHeaders = new List<Action<HttpRequestHeaders>>();
        }

        public HttpMethod Method { get; }
        public string Uri { get; }
        public HttpContent Content { get; private set; }
        public IList<Action<HttpRequestHeaders>> AddHeaders { get; }
        public Version Version { get; private set; }

        public IRestRequest WithContent(HttpContent content)
        {
            Content = content;
            return this;
        }

        public IRestRequest WithJsonContent(string content)
        {
            return WithContent(new JsonContent(content));
        }

        public IRestRequest WithHeaders(Action<HttpRequestHeaders> addHeaders)
        {
            AddHeaders.Add(addHeaders);
            return this;
        }

        public IRestRequest WithVersion(Version version)
        {
            Version = version;
            return this;
        }

        public HttpRequestMessage GetHttpRequestMessage()
        {
            var requestMessage = new HttpRequestMessage(Method, Uri);
            if (Content != null)
                requestMessage.Content = Content;
            if (Version != null)
                requestMessage.Version = Version;
            foreach (var requestAddHeader in AddHeaders)
                requestAddHeader(requestMessage.Headers);

            return requestMessage;
        }

        public override string ToString()
        {
            return $"{Method} {Uri}";
        }
    }
}
