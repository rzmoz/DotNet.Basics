using System.Web.Script.Serialization;

namespace DotNet.Basics.Rest
{
    public class JsonRestSerializer
    {
        private readonly JavaScriptSerializer _serializer;

        public JsonRestSerializer()
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
            if (@object == null)
                return "{}";

            var asString = @object as string;
            if(asString!=null)
                if(string.IsNullOrWhiteSpace(asString))
                    return "{}";
            
            return _serializer.Serialize(@object);
        }
    }
}
