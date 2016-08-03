using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Basics.RestClient
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
            if (HttpResponseMessage == null)
                return;

            StatusCode = HttpResponseMessage.StatusCode;
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
                var response = HttpResponseMessage.ToString();
                if (HttpResponseMessage.Content != null)
                    response += $"\r\n{Task.Run(() => HttpResponseMessage.Content.ReadAsStringAsync()).Result}";
                return response;
            }
            catch (Exception)
            {
                return RawContent;
            }
        }
    }
}
