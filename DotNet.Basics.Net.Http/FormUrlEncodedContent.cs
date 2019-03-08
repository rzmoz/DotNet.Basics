using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;

namespace DotNet.Basics.Net.Http
{
    public class FormUrlEncodedContent : StringContent
    {
        public const string DefaultMediaType = "application/x-www-form-urlencoded";
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content)
            : this(content, DefaultEncoding)
        {
        }

        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, Encoding encoding)
            : base(ParseContent(content), encoding, DefaultMediaType)
        {
        }

        private static string ParseContent(IEnumerable<KeyValuePair<string, string>> content)
        {
            var bodyBuilder = new StringBuilder();
            foreach (var entry in content)
                bodyBuilder.Append($"&{entry.Key}={HttpUtility.UrlEncode(entry.Value ?? string.Empty)}");
            return bodyBuilder.ToString().TrimStart('&');
        }
    }
}
