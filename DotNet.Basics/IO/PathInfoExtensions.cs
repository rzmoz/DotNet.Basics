using System;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class PathInfoExtensions
    {
        public static string FullName(this PathInfo pi)
        {
            return Paths.FileSystem.GetFullPath(pi.RawPath);
        }
        public static bool Exists(this PathInfo pi)
        {
            return pi.IsFolder ? Paths.FileSystem.ExistsDir(pi.FullName()) : Paths.FileSystem.ExistsFile(pi.FullName());
        }

        public static bool DeleteIfExists(this PathInfo pi)
        {
            return pi.DeleteIfExists(10.Seconds());
        }

        public static bool DeleteIfExists(this PathInfo pi, TimeSpan timeout)
        {
            if (pi.Exists() == false)
                return true;
            Repeat.Task(() =>
                {
                    if (pi.IsFolder)
                        Paths.FileSystem.DeleteDir(pi.FullName());
                    else
                        Paths.FileSystem.DeleteFile(pi.FullName());
                })
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 1.Seconds();
                    o.DontRethrowOnTaskFailedType = typeof(IOException);
                })
                .Until(() => pi.Exists() == false);

            return pi.Exists() == false;
        }
    }
}
