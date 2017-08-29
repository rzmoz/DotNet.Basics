using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNet.Basics.Tests.IO
{
    public class PathInfoSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.PropertyName == "IsRooted" ||
                property.PropertyName == "FullName" ||
                property.PropertyName == "NameWithoutExtension" ||
                property.PropertyName == "Extension" ||
                property.PropertyName == "RawName" ||
                property.PropertyName == "Directory" ||
                property.PropertyName == "Parent")
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
