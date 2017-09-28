using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Shell;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class IoDirPathExtensions
    {
        public static bool IsEmpty(this DirPath pi)
        {
            if (pi.Exists() == false)
                return false;
            return pi.EnumeratePaths().None();
        }

        public static bool CleanIfExists(this DirPath pi)
        {
            return pi.CleanIfExists(1.Minutes());
        }
        public static bool CleanIfExists(this DirPath pi, TimeSpan timeout)
        {
            if (pi.Exists() == false)
                return true;
            
            Parallel.ForEach(pi.EnumeratePaths(), path =>
            {
                path.DeleteIfExists();
            });

            return pi.GetPaths().Length == 0;
        }

        public static void CreateIfNotExists(this DirPath dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (dir.Exists())
                return;

            System.IO.Directory.CreateDirectory(dir.FullPath());
        }

        public static DirPath CreateSubDir(this DirPath dir, string path)
        {
            var subDir = dir.Add(path);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void ConsolidateIdenticalSubfolders(this DirPath dir, int lookDepth = int.MaxValue)
        {
            if (dir.Exists() == false)
                throw new IOException($"Directory not foud: {dir.FullPath()}");

            //depth first recursive
            if (lookDepth > 0)//we only look to a certain depth
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.ConsolidateIdenticalSubfolders(lookDepth - 1);//decrement look depth as a stop criteria
                }

            //if folder was deleted during consolidation
            if (dir.Exists() == false)
                return;

            //we move this dir up as long up the hieararchy as long as the folder names are identical
            if (dir.ParentHasIdenticalName() == false)
                return;

            bool subDirIsIdenticalToParentDir = false;

            foreach (var source in dir.GetDirectories())
            {
                var target = dir.Parent.ToDir(source.Name);
                if (target.FullPath().Equals(dir.FullPath(), StringComparison.InvariantCultureIgnoreCase))
                    subDirIsIdenticalToParentDir = true;
                Robocopy.MoveFolder(source.FullPath(), target.FullPath(), null, true);
            }

            if (subDirIsIdenticalToParentDir == false)
            {
                var source = dir;
                var target = dir.Parent;
                Robocopy.MoveFolder(source.FullPath(), target.FullPath());
            }

            //we delete the folder if it's empty if everything was moved - otherwise, we don't 
            if (dir.IsEmpty() && !subDirIsIdenticalToParentDir)
                dir.DeleteIfExists();
        }

        private static bool ParentHasIdenticalName(this DirPath dir)
        {
            if (dir.Exists() == false)
                return false;
            if (dir.Parent == null)
                return false;
            return dir.Name.Equals(dir.Parent.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void CopyTo(this DirPath source, DirPath target, bool includeSubfolders = false)
        {
            if (source.Exists() == false)
            {
                return;
            }

            if (source.FullPath().Equals(target.FullPath(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                target.CreateIfNotExists();

                //depth first to find out quickly if we have long path exceptions - we want to fail early then
                if (includeSubfolders)
                {
                    Parallel.ForEach(source.GetDirectories(), dir =>
                    {
                        var nextTargetSubDir = target.ToDir(dir.Name);
                        nextTargetSubDir.CreateIfNotExists();
                        dir.CopyTo(nextTargetSubDir, includeSubfolders);
                    });
                }

                Parallel.ForEach(source.GetFiles(), file =>
                {
                    file.CopyTo(target, overwrite: true, ensureTargetDir: false);
                });
            }
            catch (Exception e)
            {
                Robocopy.CopyDir(source.FullPath(), target.FullPath(), includeSubFolders: includeSubfolders);
            }
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
            return System.IO.Directory.GetFileSystemEntries(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToPath()).ToArray();
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
            return System.IO.Directory.EnumerateFileSystemEntries(dp.FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(fse => fse.ToPath());
        }
        private static SearchOption ToSearchOption(bool recurse)
        {
            return recurse ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;
        }
    }
}
