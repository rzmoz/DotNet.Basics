using System;
using System.IO;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class PathInfoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Segments this path "belongs" to.</param>
        /// <example>PathInfo("c:\my\dir\").Contains("dir","my") is true</example>
        /// <example>PathInfo("c:\my\dir\").Contains() is false</example>
        /// <returns></returns>
        public static bool Contains(this PathInfo pi, params string[] segments)
        {
            if (segments.Length == 0)
                return false;

            var fullName = pi.FullName.Replace('/', '\\');

            foreach (var segment in segments.Select(s => s.Replace('/', '\\')))
            {
                if (fullName.Contains(segment, StringComparison.OrdinalIgnoreCase) == false)
                    return false;
            }

            return true;
        }

        public static DirPath? ParentFromFullName(this PathInfo pi)
        {   
            return pi.Parent ?? new DirectoryInfo(pi.FullName).Parent?.Name.ToDir();
        }

        public static DirPath Directory(this PathInfo pi)
        {   
            switch (pi.PathType)
            {
                case PathType.File:
                    return pi.ParentFromFullName()!;
                default:
                    return pi.Directory;
            }
        }

        public static bool Exists(this PathInfo? pi)
        {
            if (pi == null)
                return false;
            return pi.PathType == PathType.Dir ? System.IO.Directory.Exists(pi.FullName) : File.Exists(pi.FullName);
        }

        public static bool DeleteIfExists(this PathInfo? pi)
        {
            return pi?.DeleteIfExists(20.Seconds()) ?? true;
        }

        public static bool DeleteIfExists(this PathInfo? pi, TimeSpan timeout)
        {
            if (pi.Exists() == false || pi == null)
                return true;

            Repeat.Task(() =>
                {
                    if (pi.PathType == PathType.Dir)
                        System.IO.Directory.Delete(pi.FullName, true);
                    else
                        File.Delete(pi.FullName);
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
