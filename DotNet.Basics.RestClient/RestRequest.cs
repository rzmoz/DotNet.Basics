using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.RestClient
{
    public class RestRequest : IRestRequest
    {
        private const string _urlFormat = "{0}://{1}/{2}";

        public RestRequest(string hostUrl)
            : this(hostUrl, HttpMethod.Get)
        {
        }

        public RestRequest(string hostUrl, string pathAndQuery)
            : this(ConcatenateHostUrlAndPathAndQuery(hostUrl, pathAndQuery))
        {
        }

        public RestRequest(string protocol, string domain, string pathAndQuery, HttpMethod method)
            : this(string.Format(_urlFormat, protocol, domain, pathAndQuery), method)
        {
            if (protocol == null) throw new ArgumentNullException(nameof(protocol));
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (pathAndQuery == null) throw new ArgumentNullException(nameof(pathAndQuery));
        }

        public RestRequest(string hostUrl, HttpMethod method)
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

        public Uri Uri => HttpRequestMessage.RequestUri;

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
            try
            {
                var request = HttpRequestMessage.ToString();
                if (HttpRequestMessage.Content != null)
                    request += $"\r\n{Task.Run(() => HttpRequestMessage.Content.ReadAsStringAsync()).Result}";
                return request;
            }
            catch (Exception)
            {
                return base.ToString();
            }
        }
    }
}
