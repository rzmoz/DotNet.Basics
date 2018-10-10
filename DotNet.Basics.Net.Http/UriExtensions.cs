using System;

namespace DotNet.Basics.Net.Http
{
    public static class UriExtensions
    {
        public static Uri BaseUri(this Uri uri)
        {
            return uri == null ? null : new Uri($"{uri.Scheme}://{uri.Authority}/");
        }
    }
}
