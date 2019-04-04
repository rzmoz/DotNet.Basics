using System.Reflection;
using DotNet.Basics.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class DirPathContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(PathInfo) && property.PropertyName != "RawPath")
            {
                property.ShouldSerialize = p => false;
            }

            return property;
        }
    }
}
