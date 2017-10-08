using System;

namespace DotNet.Basics.Rest
{
    public static class UriExtensions
    {
        public static Uri BaseUri(this Uri uri)
        {
            if (uri == null)
                return null;
            return new Uri($"{uri.Scheme}://{uri.Authority}/");
        }
    }
}
