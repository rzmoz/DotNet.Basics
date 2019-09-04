using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Collections;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
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

            return dp.GetPaths().Count == 0;
        }

        public static DirPath CreateIfNotExists(this DirPath dp)
        {
            if (dp.Exists() == false)
                Directory.CreateDirectory(dp.FullName());

            return dp;
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
                return;

            var targetPath = target.FullName().ToLowerInvariant();
            var sourcePath = dp.FullName().ToLowerInvariant();

            //if copy to self
            if (targetPath == sourcePath)
                return;

            if (targetPath.StartsWith(sourcePath))
                throw new IOException($"Target path is a sub path of Source path. Target: {targetPath} | Source: {sourcePath}");

            try
            {
                target.CreateIfNotExists();

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
                //switching to more robust copying if something fails
                Robocopy.CopyDir(dp.FullName(), target.FullName(), includeSubFolders: includeSubfolders);
            }
        }

        public static IReadOnlyCollection<DirPath> GetDirectories(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumerateDirectories(searchPattern ?? "*", searchOption).ToArray();
        }
        public static IReadOnlyCollection<FilePath> GetFiles(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumerateFiles(searchPattern ?? "*", searchOption).ToArray();
        }
        public static IReadOnlyCollection<PathInfo> GetPaths(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumeratePaths(searchPattern ?? "*", searchOption).ToArray();
        }
        public static IEnumerable<DirPath> EnumerateDirectories(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateDirectories(dp.FullName(), searchPattern ?? "*", searchOption).Select(dir => dir.ToDir());
        }
        public static IEnumerable<FilePath> EnumerateFiles(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateFiles(dp.FullName(), searchPattern ?? "*", searchOption).Select(file => file.ToFile());
        }
        public static IEnumerable<PathInfo> EnumeratePaths(this DirPath dp, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateFileSystemEntries(dp.FullName(), searchPattern ?? "*", searchOption).Select(fse => fse.ToPath());
        }
    }
}
