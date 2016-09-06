using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        string Serialize(object o);
        T ConvertTo<T>(object o);
    }
}
