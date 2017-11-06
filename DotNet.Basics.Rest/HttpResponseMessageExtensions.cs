using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNet.Basics.Rest
{
    public static class HttpResponseMessageExtensions
    {
        private const char _stringQuote = '\"';

        public static string Content(this HttpResponseMessage response)
        {
            return TrimQuotes(response?.Content.ReadAsStringAsync().Result);
        }
        public static T Content<T>(this HttpResponseMessage response)
        {
            var content = response.Content();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static async Task<string> ContentAsync(this HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return TrimQuotes(content);
        }
        public static async Task<T> ContentAsync<T>(this HttpResponseMessage response)
        {
            var content = response.ContentAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(await content);
        }

        public static string TrimQuotes(string content)
        {
            content = content?.Trim();
            if (content?.Length == 1)
                content = content.Trim(_stringQuote);

            if (content?.Length == 0)
                return content;

            //if quoted
            if (content?.First() == _stringQuote && content?.Last() == _stringQuote)
            {
                content = content.Substring(1);//trim lead quote
                content = content.Substring(0, content.Length - 1);
            }
            return content;
        }
    }
}
