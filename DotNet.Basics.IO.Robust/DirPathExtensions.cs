using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO.Robust
{
    public static class DirPathExtensions
    {
        public static bool IsEmpty(this DirPath dp)
        {
            return dp.Exists() && dp.EnumeratePaths().None();
        }

        public static bool CleanIfExists(this DirPath dp)
        {
            return dp.CleanIfExists(1.Minutes());
        }

        public static bool CleanIfExists(this DirPath dp, TimeSpan timeout)
        {
            if (dp.Exists() == false)
                return true;

            Parallel.ForEach(dp.EnumeratePaths(), path =>
            {
                path.DeleteIfExists();
            });

            return dp.GetPaths().Length == 0;
        }

        public static void CreateIfNotExists(this DirPath dp)
        {
            if (dp.Exists())
                return;

            System.IO.Directory.CreateDirectory(dp.FullName());
        }

        public static DirPath CreateSubDir(this DirPath dp, string subDirName)
        {
            var subDir = dp.Add(subDirName);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void CopyTo(this DirPath dp, DirPath target, bool includeSubfolders = false)
        {
            if (dp.Exists() == false)
            {
                return;
            }

            try
            {
                target.CreateIfNotExists();

                //depth first to find out quickly if we have long path exceptions - we want to fail early then
                if (includeSubfolders)
                {
                    Parallel.ForEach(dp.GetDirectories(), dir =>
                    {
                        var nextTargetSubDir = target.ToDir(dir.Name);
                        nextTargetSubDir.CreateIfNotExists();
                        dir.CopyTo(nextTargetSubDir, true);
                    });
                }

                Parallel.ForEach(dp.EnumerateFiles(), file =>
                {
                    file.CopyTo(target, overwrite: true, ensureTargetDir: false);
                });
            }
            catch (Exception)
            {
                Robocopy.CopyDir(dp.FullName(), target.FullName(), includeSubFolders: includeSubfolders);
            }
        }

        public static DirPath[] GetDirectories(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetDirectories(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir()).ToArray();
        }
        public static FilePath[] GetFiles(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFiles(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToFile()).ToArray();
        }
        public static PathInfo[] GetPaths(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFileSystemEntries(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToPath()).ToArray();
        }
        public static IEnumerable<DirPath> EnumerateDirectories(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateDirectories(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir());
        }
        public static IEnumerable<FilePath> EnumerateFiles(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFiles(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(file => file.ToFile());
        }
        public static IEnumerable<PathInfo> EnumeratePaths(this DirPath dp, string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFileSystemEntries(dp.FullName(), searchPattern ?? "*", ToSearchOption(recurse)).Select(fse => fse.ToPath());
        }
        private static SearchOption ToSearchOption(bool recurse)
        {
            return recurse ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;
        }
    }
}
