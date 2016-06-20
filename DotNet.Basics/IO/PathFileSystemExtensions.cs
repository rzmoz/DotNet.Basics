using System;
using System.Diagnostics;
using System.Management.Automation;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class PathFileSystemExtensions
    {
        public static bool DeleteIfExists(this Path path)
        {
            return DeleteIfExists(path, 30.Seconds());
        }

        public static bool IsFolder(this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (SystemIoPath.Exists(path, true))
                return true;
            if (SystemIoPath.Exists(path, false))
                return false;

            return path.EndsWith(PathDelimiterAsChar.Slash.ToString()) || path.EndsWith(PathDelimiterAsChar.Backslash.ToString());
        }

        public static bool DeleteIfExists(this Path path, TimeSpan timeout)
        {
            if (path == null)
                return false;

            if (path.Exists() == false)
                return true;

            Repeat.Task(() =>
            {
                PowerShellConsole.RemoveItem(path.FullName, force: true, recurse: true);
            })
            .IgnoreExceptionsOfType(typeof(ItemNotFoundException))
            .WithTimeout(timeout)
            .WithRetryDelay(2.Seconds())
            .Until(() => path.Exists() == false)
            .Now();

            return path.Exists() == false;
        }
    }
}
