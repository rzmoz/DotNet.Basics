using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class IoDirPathExtensions
    {
        public static bool CleanIfExists(this DirPath pi)
        {
            return pi.CleanIfExists(1.Minutes());
        }
        public static bool CleanIfExists(this DirPath pi, TimeSpan timeout)
        {
            if (pi.Exists() == false)
                return true;

            Repeat.Task(() =>
                {
                    Parallel.ForEach(pi.GetPaths(), path =>
                     {
                         try
                         {
                             path.DeleteIfExists();
                         }
                         catch (IOException)
                         { }
                     });
                })
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 2.Seconds();
                    o.DontRethrowOnTaskFailedType = typeof(IOException);
                })
                .Until(() => pi.GetPaths().Length==0);

            return pi.Exists() == false;
        }

        public static void CreateIfNotExists(this DirPath dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (dir.Exists())
                return;

            System.IO.Directory.CreateDirectory(dir.FullPath());
        }

        public static DirPath[] GetDirectories(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetDirectories(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir()).ToArray();
        }
        public static FilePath[] GetFiles(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFiles(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToFile()).ToArray();
        }
        public static PathInfo[] GetPaths(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFileSystemEntries(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => new PathInfo(dir)).ToArray();
        }
        public static IEnumerable<DirPath> EnumerateDirectories(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateDirectories(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir());
        }
        public static IEnumerable<FilePath> EnumerateFiles(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFiles(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(file => file.ToFile());
        }
        public static IEnumerable<PathInfo> EnumeratePaths(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFileSystemEntries(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(fse => new PathInfo(fse));
        }
        private static SearchOption ToSearchOption(bool recurse)
        {
            return recurse ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;
        }
    }
}
