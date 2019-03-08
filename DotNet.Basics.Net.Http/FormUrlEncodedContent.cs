using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Net.Http
{
    public class FormUrlEncodedContent : StringContent
    {
        public const string DefaultMediaType = "application/x-www-form-urlencoded";

        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, Encoding encoding = null)
            : base(content.ToQueryString(), encoding, DefaultMediaType)
        {
        }
    }
}
