using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class IoDir : IoPath<DirectoryInfo>
    {
        public IoDir(string root, params string[] paths)
            : base(root, paths)
        {
        }

        public IoDir(IoDir root, params string[] paths)
            : base(root, paths)
        {
        }

        public IoDir(DirectoryInfo root, params string[] paths)
            : base(root, paths)
        {
        }

        public IoDir Parent => FileSystemInfo.Parent.ToDir();

        public void CleanIfExists()
        {
            if (Exists() == false)
                return;
            var psc = new PowerShellConsole();
            var cleanDirScript = $@"Remove-Item ""{FullName}\*"" -recurse";
            psc.RunScript(cleanDirScript);
        }
        public void ConsolidateIdenticalSubfolders(int lookDepth = int.MaxValue)
        {
            if (Exists() == false)
                throw new IOException(FullName);

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
                if (target.FullName.Equals(FullName, StringComparison.InvariantCultureIgnoreCase))
                    subDirIsIdenticalToParentDir = true;
                Robocopy.Move(source.FullName, target.FullName);
            }

            if (subDirIsIdenticalToParentDir == false)
            {
                var filesToMove = GetFiles();
                foreach (var source in filesToMove)
                {
                    var target = Parent;
                    var moveExitCode = Cmd.Move(source.FullName, target.FullName);
                    if (moveExitCode != 0)
                        Robocopy.Move(source.Directory.FullName, target.FullName,source.Name);
                }
            }

            //we delete the folder if it's empty if everything was moved - otherwise, we don't 
            if (this.IsEmpty() && !subDirIsIdenticalToParentDir)
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

        public void CopyTo(IoDir target, DirCopyOptions dirCopyOptions)
        {
            if (Exists() == false)
            {
                Debug.WriteLine("Source '{0}' not found. Aborting", FullName);
                return;
            }

            if (FullName.Equals(target.FullName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Source and Target are the same '{0}'. Aborting", FullName);
                return;
            }

            try
            {
                //depth first to find out quickly if we have long path exceptions - we want to fail early then
                target.CreateIfNotExists();

                if (dirCopyOptions == DirCopyOptions.IncludeSubDirectories)
                {

                    Parallel.ForEach(GetDirectories(), dir =>
                    {
                        var nextTargetSubDir = target.ToDir(dir.Name);
                        nextTargetSubDir.CreateIfNotExists();
                        dir.CopyTo(nextTargetSubDir, dirCopyOptions);
                    });
                }

                Parallel.ForEach(GetFiles(), file =>
                {
                    file.CopyTo(target, FileCopyOptions.OverwriteIfExists);
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fast copy failed - falling back to use robocopy\r\n{0}", e);
                Robocopy.CopyDir(FullName, target.FullName, dirCopyOptions);
            }
        }

        public IEnumerable<IoFile> GetFiles()
        {
            return Directory.GetFiles(FullName).Select(path => path.ToFile());
        }
        public IEnumerable<IoFile> GetFiles(string searchPattern)
        {
            return Directory.GetFiles(FullName, searchPattern).Select(path => path.ToFile());
        }
        public IEnumerable<IoFile> GetFiles(string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(FullName, searchPattern, searchOption).Select(path => path.ToFile());
        }

        public IEnumerable<IoFile> EnumerateFiles()
        {
            return Directory.EnumerateFiles(FullName).Select(path => path.ToFile());
        }
        public IEnumerable<IoFile> EnumerateFiles(string searchPattern)
        {
            return Directory.EnumerateFiles(FullName, searchPattern).Select(path => path.ToFile());
        }
        public IEnumerable<IoFile> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(FullName, searchPattern, searchOption).Select(path => path.ToFile());
        }

        public IEnumerable<IoDir> GetDirectories()
        {
            return Directory.GetDirectories(FullName).Select(path => path.ToDir());
        }
        public IEnumerable<IoDir> GetDirectories(string searchPattern)
        {
            return Directory.GetDirectories(FullName, searchPattern).Select(path => path.ToDir());
        }
        public IEnumerable<IoDir> GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(FullName, searchPattern, searchOption).Select(path => path.ToDir());
        }

        public IEnumerable<IoDir> EnumerateDirectories()
        {
            return Directory.EnumerateDirectories(FullName).Select(path => path.ToDir());
        }
        public IEnumerable<IoDir> EnumerateDirectories(string searchPattern)
        {
            return Directory.EnumerateDirectories(FullName, searchPattern).Select(path => path.ToDir());
        }
        public IEnumerable<IoDir> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(FullName, searchPattern, searchOption).Select(path => path.ToDir());
        }
    }
}
