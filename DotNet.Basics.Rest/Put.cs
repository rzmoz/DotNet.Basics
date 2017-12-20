using System.Net.Http;

namespace DotNet.Basics.Rest
{
    public static class Put
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Put, uri);
        }
    }
}
