using System.Net.Http;

namespace DotNet.Standard.Rest
{
    public static class Post
    {
        public static IRestRequest Uri(string uri)
        {
            return new RestRequest(HttpMethod.Post, uri);
        }
    }
}
