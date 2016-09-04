using System;
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
            .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 2.Seconds();
                    o.IgnoreExceptionType = typeof(ItemNotFoundException);
                })
            .Until(() => path.Exists() == false);

            return path.Exists() == false;
        }
    }
}
