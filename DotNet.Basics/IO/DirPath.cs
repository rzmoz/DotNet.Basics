using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class DirPath : PathInfo
    {
        public DirPath(string path, params string[] segments) : this(path, PathSeparator.Unknown, segments)
        {
        }

        public DirPath(string path, char pathSeparator, params string[] segments)
            : base(path, IO.IsFolder.True, pathSeparator, segments)
        {
        }

        public bool IsEmpty()
        {
            return Exists() && EnumeratePaths().None();
        }

        public bool CleanIfExists()
        {
            return CleanIfExists(1.Minutes());
        }
        public bool CleanIfExists(TimeSpan timeout)
        {
            if (Exists() == false)
                return true;

            Parallel.ForEach(EnumeratePaths(), path =>
            {
                path.DeleteIfExists();
            });

            return GetPaths().Length == 0;
        }

        public void CreateIfNotExists()
        {
            if (Exists())
                return;

            System.IO.Directory.CreateDirectory(FullPath());
        }

        public DirPath CreateSubDir(string path)
        {
            var subDir = this.Add(path);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public void ConsolidateIdenticalSubfolders(int lookDepth = int.MaxValue)
        {
            if (Exists() == false)
                throw new IOException($"Directory not foud: {FullPath()}");

            //depth first recursive
            if (lookDepth > 0)//we only look to a certain depth
                foreach (var subDir in GetDirectories())
                {
                    subDir.ConsolidateIdenticalSubfolders(lookDepth - 1);//decrement look depth as a stop criteria
                }

            //if folder was deleted during consolidation
            if (Exists() == false)
                return;

            //we move this dir up as long up the hieararchy as long as the folder names are identical
            if (ParentHasIdenticalName() == false)
                return;

            bool subDirIsIdenticalToParentDir = false;

            foreach (var source in GetDirectories())
            {
                var target = Parent.ToDir(source.Name);
                if (target.FullPath().Equals(FullPath(), StringComparison.InvariantCultureIgnoreCase))
                    subDirIsIdenticalToParentDir = true;
                Robocopy.MoveFolder(source.FullPath(), target.FullPath(), null, true);
            }

            if (subDirIsIdenticalToParentDir == false)
            {
                Robocopy.MoveFolder(this.FullPath(), Parent.FullPath());
            }

            //we delete the folder if it's empty if everything was moved - otherwise, we don't 
            if (IsEmpty() && !subDirIsIdenticalToParentDir)
                DeleteIfExists();
        }

        private bool ParentHasIdenticalName()
        {
            if (Exists() == false)
                return false;
            if (Parent == null)
                return false;
            return Name.Equals(Parent.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public void CopyTo(DirPath target, bool includeSubfolders = false)
        {
            if (Exists() == false)
            {
                return;
            }

            if (FullPath().Equals(target.FullPath(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                target.CreateIfNotExists();

                //depth first to find out quickly if we have long path exceptions - we want to fail early then
                if (includeSubfolders)
                {
                    Parallel.ForEach(GetDirectories(), dir =>
                    {
                        var nextTargetSubDir = target.ToDir(dir.Name);
                        nextTargetSubDir.CreateIfNotExists();
                        dir.CopyTo(nextTargetSubDir, includeSubfolders);
                    });
                }

                Parallel.ForEach(GetFiles(), file =>
                {
                    file.CopyTo(target, overwrite: true, ensureTargetDir: false);
                });
            }
            catch (Exception)
            {
                Robocopy.CopyDir(FullPath(), target.FullPath(), includeSubFolders: includeSubfolders);
            }
        }

        public DirPath[] GetDirectories(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetDirectories(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir()).ToArray();
        }
        public FilePath[] GetFiles(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFiles(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToFile()).ToArray();
        }
        public PathInfo[] GetPaths(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFileSystemEntries(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToPath()).ToArray();
        }
        public IEnumerable<DirPath> EnumerateDirectories(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateDirectories(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir());
        }
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFiles(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(file => file.ToFile());
        }
        public IEnumerable<PathInfo> EnumeratePaths(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFileSystemEntries(FullPath(), searchPattern ?? "*", ToSearchOption(recurse)).Select(fse => fse.ToPath());
        }
        private static SearchOption ToSearchOption(bool recurse)
        {
            return recurse ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;
        }
    }
}
