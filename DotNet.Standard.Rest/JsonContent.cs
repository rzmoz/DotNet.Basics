using System.Net.Http;
using System.Text;

namespace DotNet.Standard.Rest
{
    public class JsonContent : StringContent
    {
        private const string _defaultContentType = "application/json";

        public JsonContent(string json) : this(json, Encoding.UTF8)
        { }

        public JsonContent(string json, Encoding encoding) : base(json, encoding, _defaultContentType)
        { }
    }
}
