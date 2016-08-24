using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Rest
{
    public class JsonContent : StringContent
    {
        private static readonly JsonRestSerializer _serializer = new JsonRestSerializer();

        public const string ContentType = "application/json";

        public JsonContent(string json) : base(json, Encoding.UTF8, ContentType)
        {
        }
        public JsonContent(string json, Encoding encoding) : base(json, encoding, ContentType)
        { }

        public JsonContent(object content) : base(SerializeContent(content), Encoding.UTF8, ContentType)
        {
        }

        public JsonContent(object content, Encoding encoding) : base(SerializeContent(content), encoding, ContentType)
        {

        }

        private static string SerializeContent(object content)
        {
            if (content == null)
                return "{}";

            return _serializer.Serialize(content);
        }
    }
}
