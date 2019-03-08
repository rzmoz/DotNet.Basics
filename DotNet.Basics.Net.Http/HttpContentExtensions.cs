using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace DotNet.Basics.Net.Http
{
    public static class HttpContentExtensions
    {
        private const char _stringQuote = '\"';

        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var bodyBuilder = new StringBuilder();
            foreach (var entry in keyValues)
                bodyBuilder.Append($"&{entry.Key}={HttpUtility.UrlEncode(entry.Value ?? string.Empty)}");
            return bodyBuilder.ToString().TrimStart('&');
        }

        public static async Task<T> ReadAsTypeAsync<T>(this HttpContent content, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var json = await content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
        }

        public static string TrimQuotes(string content)
        {
            if (content == null)
                return null;

            content = content.Trim();
            if (content.Length == 1)
                content = content.Trim(_stringQuote);

            if (content.Length == 0)
                return content;

            //if quoted
            if (content.First() == _stringQuote && content.Last() == _stringQuote)
            {
                content = content.Substring(1);//trim lead quote
                content = content.Substring(0, content.Length - 1);
            }
            return content;
        }
    }
}
