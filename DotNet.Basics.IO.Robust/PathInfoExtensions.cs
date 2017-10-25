using System;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO.Robust
{
    public static class PathInfoExtensions
    {
        public static string FullName(this PathInfo pi)
        {
            return NetCoreLongPath.NormalizePath(pi.RawPath);
        }
        public static bool Exists(this PathInfo pi, params string[] segments)
        {
            var combinedPath = pi;
            if (segments?.Length > 0)
                combinedPath = combinedPath.RawPath.ToPath(segments);
            return NetCoreLongPath.Exists(combinedPath.FullName());
        }
        public static bool DeleteIfExists(this PathInfo pi)
        {
            return pi.DeleteIfExists(10.Seconds());
        }

        public static bool DeleteIfExists(this PathInfo pi, TimeSpan timeout)
        {
            if (pi.Exists() == false)
                return true;
            var fullName = pi.FullName();
            Repeat.Task(() =>
                {
                    try
                    {
                        NetCoreLongPath.DeleteDir(fullName);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    try
                    {
                        NetCoreLongPath.DeleteFile(fullName);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                })
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 2.Seconds();
                    o.DontRethrowOnTaskFailedType = typeof(IOException);
                })
                .Until(() => pi.Exists() == false);

            return pi.Exists() == false;
        }
    }
}
