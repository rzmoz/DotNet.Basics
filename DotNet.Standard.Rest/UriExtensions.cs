using System;

namespace DotNet.Standard.Rest
{
    public static class UriExtensions
    {
        public static Uri BaseUri(this Uri uri)
        {
            return uri == null ? null : new Uri($"{uri.Scheme}://{uri.Authority}/");
        }
    }
}
