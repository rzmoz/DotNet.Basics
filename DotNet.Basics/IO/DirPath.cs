using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

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

        protected override void InternalDeleteIfExists()
        {
#if NETSTANDARD2_0
            NetStandardIoPath.TryDeleteDir(FullPath());
#endif
#if NET47
                NetFrameworkIoPath.TryDeleteDir(FullPath(), IsFolder);
#endif

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
