using System.Net.Http;

namespace DotNet.Standard.Rest
{
    public static class Head
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Head, uri);
        }
    }
}
