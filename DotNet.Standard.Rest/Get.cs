using System.Net.Http;

namespace DotNet.Standard.Rest
{
    public static class Get
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Get, uri);
        }
    }
}
