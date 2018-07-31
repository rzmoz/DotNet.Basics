using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNet.Basics.Rest
{
    public static class HttpContentExtensions
    {
        private const char _stringQuote = '\"';

        public static async Task<T> ReadAsTypeAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json);
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
