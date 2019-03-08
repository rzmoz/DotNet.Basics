using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DotNet.Basics.Net.Http
{
    public class JsonContent : StringContent
    {
        public const string DefaultMediaType = "application/json";

        public JsonContent(object obj, Encoding encoding = null) : this(JsonConvert.SerializeObject(obj), encoding)
        { }

        public JsonContent(string json, Encoding encoding = null) : base(json, encoding, DefaultMediaType)
        { }
    }
}
