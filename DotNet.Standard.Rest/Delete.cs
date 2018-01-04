using System.Net.Http;

namespace DotNet.Standard.Rest
{
    public static class Delete
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Delete, uri);
        }
    }
}
