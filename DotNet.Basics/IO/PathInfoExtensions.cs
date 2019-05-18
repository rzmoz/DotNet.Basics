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
            return Path.GetFullPath(pi.RawPath);
        }

        public static DirPath ParentFromFullName(this PathInfo pi)
        {
            if (pi == null)
                return null;

            return pi.Parent ?? new DirectoryInfo(pi.FullName()).Parent?.Name.ToDir();
        }

        public static DirPath Directory(this PathInfo pi)
        {
            if (pi == null)
                return null;

            switch (pi.PathType)
            {
                case PathType.File:
                    return pi.ParentFromFullName();
                default:
                    return pi.Directory;
            }
        }

        public static bool Exists(this PathInfo pi)
        {
            return pi.PathType == PathType.Dir ? System.IO.Directory.Exists(pi.FullName()) : System.IO.File.Exists(pi.FullName());
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
                    if (pi.PathType == PathType.Dir)
                        System.IO.Directory.Delete(pi.FullName(), true);
                    else
                        System.IO.File.Delete(pi.FullName());
                })
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 1.Seconds();
                    o.MuteExceptions.Add<IOException>();
                })
                .Until(() => pi.Exists() == false);

            return pi.Exists() == false;
        }
    }
}
