using System.Net.Http;

namespace DotNet.Basics.Rest
{
    public static class Head
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Head, uri);
        }
    }
}
