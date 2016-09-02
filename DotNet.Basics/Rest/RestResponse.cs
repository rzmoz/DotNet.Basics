using System;
using System.Net;
using System.Net.Http;

namespace DotNet.Basics.Rest
{
    public class RestResponse<T> : IRestResponse<T>
    {
        private const char _stringQuote = '\"';

        public RestResponse(Uri requestUri, HttpResponseMessage httpResponseMessage = null, ResponseFormatting responseFormatting = ResponseFormatting.Raw)
        {
            Exception = null;
            Uri = requestUri;
            HttpResponseMessage = httpResponseMessage;
            StatusCode = 0;
            ReasonPhrase = string.Empty;
            if (HttpResponseMessage == null)
                return;

            StatusCode = HttpResponseMessage.StatusCode;
            ReasonPhrase = HttpResponseMessage.ReasonPhrase;
            ResponseContent = HttpResponseMessage.Content?.ReadAsStringAsync().Result;
            var serializer = new JsonRestSerializer();

            RawContent = serializer.ConvertTo<string>(ResponseContent);

            if (typeof(T) == typeof(string))
            {
                if (responseFormatting == ResponseFormatting.TrimQuotesWhenContentIsString)
                    ResponseContent = TrimQuotesInString(ResponseContent);
                Content = serializer.ConvertTo<T>(ResponseContent);
            }
            else
                Content = serializer.FromJson<T>(ResponseContent);
        }

        public Uri Uri { get; }
        public HttpStatusCode StatusCode { get; }
        public string ReasonPhrase { get; }
        public T Content { get; }
        public string RawContent { get; }

        public Exception Exception { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; }

        private string ResponseContent { get; }

        private string TrimQuotesInString(string responseContent)
        {
            if (string.IsNullOrEmpty(responseContent))
                return responseContent;

            //ensure we trim away only a single quote in case the innner string has actual quotes
            var trimmedContent = responseContent.TrimStart(_stringQuote);
            trimmedContent = trimmedContent.PadLeft(responseContent.Length - 1, _stringQuote);
            responseContent = trimmedContent;
            trimmedContent = responseContent.TrimEnd(_stringQuote);
            trimmedContent = trimmedContent.PadRight(responseContent.Length - 1, _stringQuote);
            return trimmedContent;
        }

        public override string ToString()
        {
            try
            {
                return $"{HttpResponseMessage}\r\n{HttpResponseMessage.Content?.ReadAsStringAsync().Result}";
            }
            catch (Exception)
            {

                return HttpResponseMessage.ToString();
            }
        }
    }
}
