using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Rest
{
public static    class Put
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Put, uri);
        }
    }
}
