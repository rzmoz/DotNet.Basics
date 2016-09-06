using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace DotNet.Basics.Rest
{
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JavaScriptSerializer _jsSerializer=new JavaScriptSerializer();

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object o)
        {
            if (o == null || (o is string && string.IsNullOrWhiteSpace(o.ToString())))
                return "{}";
            return JsonConvert.SerializeObject(o, Formatting.None).Trim('"');
        }

        public T ConvertTo<T>(object o)
        {
            return _jsSerializer.ConvertToType<T>(o);
        }
    }
}
