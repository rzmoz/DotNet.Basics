using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Net.Http
{
    public class JsonContent : StringContent
    {
        public const string DefaultMediaType = "application/json";

        public JsonContent(string json, Encoding encoding = null) : base(json, encoding, DefaultMediaType)
        { }
    }
}
