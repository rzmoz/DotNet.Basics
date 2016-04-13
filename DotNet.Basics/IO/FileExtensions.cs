using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static IoFile ToFile(this string root, params string[] paths)
        {
            return new IoFile(root, paths);
        }
        public static IoFile ToFile(this IoDir dir, string fileName)
        {
            return new IoFile(dir, fileName);
        }

        public static IoFile ToFile(this IoDir dir, IoFile fileName)
        {
            return new IoFile(dir, fileName.Name);
        }

        public static void CopyTo(this IEnumerable<IoFile> sourceFiles, IoDir targetDir, FileCopyOptions fileCopyOptions = FileCopyOptions.AbortIfExists)
        {
            Parallel.ForEach(sourceFiles, (file) => file.CopyTo(targetDir, fileCopyOptions));
        }

        public static string ToPath(this IEnumerable<string> paths)
        {
            var pathsList = paths.Select(CleanPath).ToList();
            return Path.Combine(pathsList.ToArray());
        }

        public static string ToPath(this string root, params string[] paths)
        {
            return ToPath(new[] {root}, paths);
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