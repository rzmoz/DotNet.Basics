using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Rest
{
    public class JsonContent : StringContent
    {
        private static readonly JsonRestSerializer _serializer = new JsonRestSerializer();

        private const string _cdefaultContentType = "application/json";

        public JsonContent(string json) : this(json, Encoding.UTF8)
        {
        }

        public JsonContent(string json, Encoding encoding) : this(json, encoding, _cdefaultContentType)
        {
        }

        public JsonContent(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
        {
        }

        public JsonContent(object content) : this(SerializeContent(content))
        {
        }

        public JsonContent(object content, Encoding encoding) : this(SerializeContent(content), encoding)
        {
        }

        public JsonContent(object content, Encoding encoding, string mediaType) : this(SerializeContent(content), encoding, mediaType)
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
