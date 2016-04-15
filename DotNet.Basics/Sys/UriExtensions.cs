using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNet.Basics.Sys
{
    public static class UriExtensions
    {
        public static Uri HostUriOnly(this Uri uri)
        {
            var fullHost = string.Format(_fullHostFormat, uri.Scheme, uri.Host, uri.Port);
            return new Uri(fullHost);
        }

        public static Uri ReplaceHost(this Uri uri, string newHost)
        {
            return new Uri(new Uri(newHost), uri.PathAndQuery);
        }

        public static Uri EnsurePathPrefix(this Uri uri, string pathPrefix)
        {
            var trimmedPrefix = pathPrefix.Trim(_pathSeparator);
            trimmedPrefix = trimmedPrefix.EnsurePostfix(_pathSeparator.ToString());

            var pathAndQuery = uri.PathAndQuery.TrimStart(_pathSeparator);

            var prefixedathAndQuery = pathAndQuery.EnsurePrefix(trimmedPrefix);
            var hostUri = uri.HostUriOnly();
            return new Uri(hostUri, prefixedathAndQuery);
        }

        public static Uri RemovePathPrefix(this Uri uri, string pathPrefix)
        {
            var newPathAndQuery = uri.PathAndQuery.RemovePrefix("/");
            newPathAndQuery = newPathAndQuery.RemovePrefix(pathPrefix);
            newPathAndQuery = newPathAndQuery.EnsurePrefix("/");
            return new Uri(uri.HostUriOnly(), newPathAndQuery);
        }

        public static Uri ReplacePathAndQuery(this Uri uri, string newPathAndQuery)
        {
            return new Uri(uri.HostUriOnly(), newPathAndQuery);
        }

        public static string UrlCombine(this string root, params string[] paths)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (paths == null) throw new ArgumentNullException(nameof(paths));
            if (paths.Length == 0)
                return root;

            var concatenatedPaths = new List<string> { root };
            concatenatedPaths.AddRange(paths.Select(path => path.TrimStart(_pathSeparator)));

            var combined = Path.Combine(concatenatedPaths.ToArray());

            return combined.Replace('\\', _pathSeparator);
        }

        private const char _pathSeparator = '/';
        private const string _fullHostFormat = "{0}://{1}:{2}";
    }
}
