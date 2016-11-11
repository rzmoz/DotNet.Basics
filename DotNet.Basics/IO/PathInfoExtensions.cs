using System;
using System.Collections.Generic;
using System.Management.Automation;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class PathInfoExtensions
    {
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return new PathInfo(path, segments);
        }

        public static PathInfo ToPath(this IReadOnlyCollection<string> pathSegments, bool isFolder)
        {
            if (isFolder)
                return new DirPath(pathSegments);
            return new FilePath(pathSegments);
        }

        public static bool Exists(this PathInfo pi, bool throwIoExceptionIfNotExists = false)
        {
            return SystemIoPath.Exists(pi.FullName, pi.IsFolder, throwIoExceptionIfNotExists);
        }

        public static bool DeleteIfExists(this PathInfo path)
        {
            return DeleteIfExists(path, 30.Seconds());
        }

        public static bool DeleteIfExists(this PathInfo path, TimeSpan timeout)
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
                o.DontRethrowOnTaskFailedType = typeof(ItemNotFoundException);
            })
            .Until(() => path.Exists() == false);

            return path.Exists() == false;
        }
    }
}
