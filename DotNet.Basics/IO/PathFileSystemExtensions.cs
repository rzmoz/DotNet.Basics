using System;
using System.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class PathFileSystemExtensions
    {
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
            return SystemIoPath.Exists(path);
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
    }
}
