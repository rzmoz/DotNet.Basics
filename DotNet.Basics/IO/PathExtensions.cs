using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {

        public static string ToPath(this IEnumerable<string> paths)
        {
            var pathsList = paths.Select(CleanPath).ToList();
            return Path.Combine(pathsList.ToArray());
        }

        public static string ToPath(this FileSystemInfo root, params string[] paths)
        {
            return root.FullName.ToPath(paths);
        }

        public static string ToPath(this string root, params string[] paths)
        {
            return ToPath(new[] { root }, paths);
        }

        public static string ToPath(this IEnumerable<string> root, params string[] paths)
        {
            var allPaths = new List<string>();
            if (root != null)
                allPaths.AddRange(root);
            if (paths.Any())
                allPaths.AddRange(paths);
            return ToPath(allPaths.ToArray());
        }

        private static string CleanPath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return string.Empty;
            return rawPath.Trim('/').Trim('\\');
        }
    }
}
