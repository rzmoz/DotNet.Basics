using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Net.Http
{
    public static class HttpMessageExtensions
    {
        private const char _stringQuote = '\"';

        public static Task<string> ToDebugOutputAsync(this IRestRequest request)
        {
            return request.HttpRequestMessage.ToDebugOutputAsync();
        }
        public static async Task<string> ToDebugOutputAsync(this HttpRequestMessage msg)
        {
            var debug = new StringBuilder();
            debug.AppendLine($"Uri : {msg.RequestUri}");
            debug.AppendLine($"Headers :");
            foreach (var header in msg.Headers)
                foreach (var value in header.Value)
                    debug.AppendLine($"{header.Key}:{value}");
            debug.AppendLine($"Body :");
            if (msg.Content != null)
                debug.AppendLine(await msg.ContentAsync().ConfigureAwait(false));
            else
                debug.AppendLine("<EMPTY>");
            return debug.ToString();
        }

        public static async Task<string> ToDebugOutputAsync(this HttpResponseMessage msg)
        {
            var debug = new StringBuilder();
            debug.AppendLine($"{(int)msg.StatusCode} {msg.ReasonPhrase}");
            debug.AppendLine($"Headers :");
            foreach (var header in msg.Headers)
                foreach (var value in header.Value)
                    debug.AppendLine($"{header.Key}:{value}");
            debug.AppendLine($"Body :");
            if (msg.Content != null)
                debug.AppendLine(await msg.ContentAsync().ConfigureAwait(false));
            else
                debug.AppendLine("<EMPTY>");
            return debug.ToString();
        }

        public static async Task<string> ContentAsync(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            return TrimQuotes(content);
        }

        public static async Task<string> ContentAsync(this HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return TrimQuotes(content);
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
