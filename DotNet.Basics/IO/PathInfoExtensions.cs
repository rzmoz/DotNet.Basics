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
            try
            {
                if (Paths.FileSystem.ExistsDir(pi.FullName()))
                    return true;
            }
            catch (IOException)
            {
                //ignored
            }
            try
            {
                if (Paths.FileSystem.ExistsFile(pi.FullName()))
                    return true;
            }
            catch (IOException)
            {
                //ignored
            }
            return false;
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
                        Paths.FileSystem.DeleteDir(fullName);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    try
                    {
                        Paths.FileSystem.DeleteFile(fullName);
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
