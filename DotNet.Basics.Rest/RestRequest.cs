using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public class RestRequest : IRestRequest
    {
        public RestRequest(string uri)
            : this(uri, HttpMethod.Get)
        {
        }
        public RestRequest(string baseUrl, string pathAndQuery)
            : this(CombineBaseUrlAndPathAndQuery(baseUrl, pathAndQuery))
        {
        }
        public RestRequest(string baseUrl, string pathAndQuery, HttpMethod method)
            : this(CombineBaseUrlAndPathAndQuery(baseUrl, pathAndQuery), method)
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

        public HttpRequestMessage HttpRequestMessage { get; }

        public Uri Uri
        {
            get => HttpRequestMessage.RequestUri;
            set => HttpRequestMessage.RequestUri = value;
        }

        public HttpMethod Method
        {
            get => HttpRequestMessage.Method;
            set => HttpRequestMessage.Method = value;
        }
        public HttpRequestHeaders Headers => HttpRequestMessage.Headers;

        public HttpContent Content
        {
            get => HttpRequestMessage.Content;
            set => HttpRequestMessage.Content = value;
        }
        public bool DisableCertificateValidation { get; set; }
        public TimeSpan TimeOut { get; set; }

        public override string ToString()
        {
            try
            {
                return $"{HttpRequestMessage}\r\n{HttpRequestMessage.Content?.ReadAsStringAsync().Result}";
            }
            catch (Exception)
            {

                return HttpRequestMessage.ToString();
            }
        }

        private static string CombineBaseUrlAndPathAndQuery(string baseUrl, string pathAndQuery)
        {
            if (baseUrl == null) throw new ArgumentNullException(nameof(baseUrl));
            if (pathAndQuery == null) throw new ArgumentNullException(nameof(pathAndQuery));
            return baseUrl.TrimEnd('/') + "/" + pathAndQuery.TrimStart('/');
        }
    }
}
