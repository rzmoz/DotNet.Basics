using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DotNet.Basics.IO;

namespace DotNet.Basics.Rest
{
    public class RestRequest : IRestRequest
    {
        public RestRequest(string uri)
            : this(uri, HttpMethod.Get)
        {
        }
        public RestRequest(string baseUrl, string pathAndQuery)
            : this(baseUrl.ToPath(pathAndQuery).FullName)
        {
        }
        public RestRequest(string baseUrl, string pathAndQuery, HttpMethod method)
            : this(baseUrl.ToPath(pathAndQuery).FullName, method)
        {
        }

        public RestRequest(string scheme, string authority, string pathAndQuery, HttpMethod method)
            : this(new Uri($"{scheme}://{authority}/{pathAndQuery}"), method)
        {
        }

        public RestRequest(Uri uri)
            : this(uri, HttpMethod.Get)
        {
        }
        public RestRequest(Uri baseUri, string relativeUri)
            : this(new Uri(baseUri, relativeUri), HttpMethod.Get)
        {
        }
        public RestRequest(Uri baseUri, string relativeUri, HttpMethod method)
            : this(new Uri(baseUri, relativeUri), method)
        {
        }

        public RestRequest(string hostUrl, HttpMethod method)
            : this(new HttpRequestMessage(method, hostUrl))
        {
        }
        public RestRequest(Uri hostUrl, HttpMethod method)
            : this(new HttpRequestMessage(method, hostUrl))
        {
        }

        public RestRequest(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException(nameof(httpRequestMessage));
            HttpRequestMessage = httpRequestMessage;
            DisableCertificateValidation = false;
            TimeOut = TimeSpan.FromHours(1);//we default to 1 hour timeout            
        }

        private static string ConcatenateHostUrlAndPathAndQuery(string hostUrl, string pathAndQuery)
        {
            if (hostUrl == null) throw new ArgumentNullException(nameof(hostUrl));
            if (pathAndQuery == null) throw new ArgumentNullException(nameof(pathAndQuery));

            var url = $"{hostUrl}/{pathAndQuery}";

            const string doubleSlashes = "//";

            while (url.LastIndexOf(doubleSlashes, StringComparison.Ordinal) != url.IndexOf(doubleSlashes, StringComparison.Ordinal))
                url = url.Remove(url.LastIndexOf(doubleSlashes, System.StringComparison.Ordinal), 1);
            return url;
        }

        public HttpRequestMessage HttpRequestMessage { get; }

        public Uri Uri
        {
            get { return HttpRequestMessage.RequestUri; }
            set { HttpRequestMessage.RequestUri = value; }
        }

        public HttpMethod Method
        {
            get { return HttpRequestMessage.Method; }
            set { HttpRequestMessage.Method = value; }
        }
        public HttpRequestHeaders Headers => HttpRequestMessage.Headers;

        public HttpContent Content
        {
            get { return HttpRequestMessage.Content; }
            set { HttpRequestMessage.Content = value; }
        }
        public bool DisableCertificateValidation { get; set; }
        public TimeSpan TimeOut { get; set; }

        public override string ToString()
        {
            return HttpRequestMessage.ToString();
        }
    }
}
