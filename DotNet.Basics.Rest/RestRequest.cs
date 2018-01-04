using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        public HttpMethod Method { get; private set; }
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

        public Task<HttpResponseMessage> SendAsync(IRestClient client)
        {
            var fullUri = client.BaseAddress == null ? new Uri(Uri) : new Uri(client.BaseAddress, Uri);

            var requestMessage = new HttpRequestMessage(Method, fullUri);
            if (Content != null)
                Content = Content;
            if (Version != null)
                Version = Version;
            foreach (var addHeader in AddHeaders)
                addHeader?.Invoke(requestMessage.Headers);

            return client.SendAsync(requestMessage);
        }

        public override string ToString()
        {
            return $"{Method} {Uri}";
        }
    }
}
