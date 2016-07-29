using System;
using System.Web.Script.Serialization;

namespace DotNet.Basics.Net
{
    public class JsonSerializer
    {
        private readonly JavaScriptSerializer _serializer;

        public JsonSerializer()
        {
            _serializer = new JavaScriptSerializer();
        }

        public T ConvertTo<T>(string json)
        {
            return _serializer.ConvertToType<T>(json);

        }
        public T FromJson<T>(string json)
        {

            return _serializer.Deserialize<T>(json);
        }

        public string Serialize(object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            return _serializer.Serialize(@object);
        }
    }
}
