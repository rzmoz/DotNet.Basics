using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DotNet.Basics.Rest
{
    public class JsonContent : StringContent
    {
        public const string DefaultContentType = "application/json";
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public JsonContent(object obj) : this(JsonConvert.SerializeObject(obj), DefaultEncoding)
        { }

        public JsonContent(object obj, Encoding encoding) : this(JsonConvert.SerializeObject(obj), encoding)
        { }

        public JsonContent(string json) : this(json, DefaultEncoding)
        { }

        public JsonContent(string json, Encoding encoding) : base(json, encoding, DefaultContentType)
        { }
    }
}
