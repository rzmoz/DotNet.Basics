using System.Net.Http;
using System.Text;
using ServiceStack.Text;

namespace DotNet.Basics.Rest
{
    public class JsonContent : StringContent
    {
        public const string DefaultContentType = "application/json";
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public JsonContent(object obj) : this(JsonSerializer.SerializeToString(obj), DefaultEncoding)
        { }

        public JsonContent(object obj, Encoding encoding) : this(JsonSerializer.SerializeToString(obj), encoding)
        { }

        public JsonContent(string json) : this(json, DefaultEncoding)
        { }

        public JsonContent(string json, Encoding encoding) : base(json, encoding, DefaultContentType)
        { }
    }
}
