using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        private const char _slashDelimiter = '/';
        private const char _backslashDelimiter = '\\';

        public static void CleanIfExists(this Path path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (path.IsFolder == false)
                throw new PathException($"Can't clean path because it's not a folder", path);
            PowerShellConsole.RemoveItem($"{path.FullName}\\*", force: true, recurse: true);
        }

        public static void CreateIfNotExists(this Path path)
        {
            if (path.Exists())
                return;

            if (path.IsFolder == false)
                throw new PathException($"Can't create path because it's not a folder {path}", path);

            PowerShellConsole.NewItem(path.FullName, "Directory", false);
            Debug.WriteLine($"Created: {path.FullName}");
        }

        public static bool Exists(this Path path)
        {
            if (path == null)
                return false;
            var result = PowerShellConsole.RunScript($"Test-Path -Path '{path.FullName.Trim()}' -pathtype any");
            return result.Single().ToString().Equals("True", StringComparison.OrdinalIgnoreCase);
        }


        public static bool DeleteIfExists(this Path path)
        {
            return DeleteIfExists(path, 30.Seconds());
        }
        public static bool DeleteIfExists(this Path path, TimeSpan timeout)
        {
            if (path == null)
                return false;

            Repeat.Task(() =>
            {
                PowerShellConsole.RemoveItem(path.FullName, force: true, recurse: true);
            })
            .WithTimeout(timeout)
            .WithRetryDelay(3.Seconds())
            .Until(() => path.Exists() == false)
            .Now();

            return path.Exists() == false;
        }


        public static PathDelimiter ToPathDelimiter(this char delimiter)
        {
            switch (delimiter)
            {
                case _backslashDelimiter:
                    return PathDelimiter.Backslash;
                case _slashDelimiter:
                    return PathDelimiter.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {delimiter}");
            }
        }

        public static char ToChar(this PathDelimiter pathDelimiter)
        {
            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return _backslashDelimiter;
                case PathDelimiter.Slash:
                    return _slashDelimiter;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }

        public static Path ToPath(this FileInfo file)
        {
            return new Path(file.FullName, DetectOptions.SetToFile);
        }
        public static Path ToPath(this DirectoryInfo dir)
        {
            return new Path(dir.FullName, DetectOptions.SetToDir);
        }
        public static Path ToPath(this string root, params string[] paths)
        {
            return root.ToPath(DetectOptions.AutoDetect, paths);
        }
        public static Path ToPath(this string root, DetectOptions detectOptions, params string[] paths)
        {
            return new Path(root, detectOptions).Add(detectOptions, paths);
        }
        public static Path ToPath(this string root, PathDelimiter delimiter, params string[] paths)
        {
            var path = new Path(root).Add(paths);
            path.Delimiter = delimiter;
            return path;
        }
        public static Path ToPath(this string root, DetectOptions detectOptions, PathDelimiter delimiter, params string[] paths)
        {
            var path = new Path(root).Add(detectOptions, paths);
            path.Delimiter = delimiter;
            return path;
        }
    }
}
