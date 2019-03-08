﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace DotNet.Basics.Net.Http
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

        public IRestRequest WithFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, Encoding encoding = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return WithHttpContent(new FormUrlEncodedContent(content));
        }
        public IRestRequest WithJsonContent(string content)
        {
            return WithHttpContent(new JsonContent(content));
        }
        public IRestRequest WithStringContent(string content, string mediaType = JsonContent.DefaultMediaType, Encoding encoding = null)
        {
            return WithHttpContent(new StringContent(content, encoding, mediaType));
        }
        public IRestRequest WithHttpContent(HttpContent content)
        {
            if (content != null)
                HttpRequestMessage.Content = content;
            return this;
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
