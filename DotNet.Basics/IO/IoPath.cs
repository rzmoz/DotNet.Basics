using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public abstract class IoPath
    {
        private readonly Func<DirectoryInfo> GetDir;
        private readonly Action Delete;

        protected IoPath(FileSystemInfo fsi)
        {
            if (fsi == null) throw new ArgumentNullException(nameof(fsi));
            FileSystemInfo = fsi;

            var fi = fsi as FileInfo;
            if (fi != null)
            {
                GetDir = () => fi.Directory;
                Delete = () =>
                {
                    try
                    {
                        File.Delete(FileSystemInfo.FullName);
                    }
                    finally
                    {
                        FileSystemInfo.Refresh();
                    }
                };
            }


            var di = fsi as DirectoryInfo;
            if (di != null)
            {
                GetDir = () => di;
                Delete = () =>
                {
                    try
                    {
                        Directory.Delete(FileSystemInfo.FullName, true);
                    }
                    finally
                    {
                        FileSystemInfo.Refresh();
                    }
                };
            }
        }

        public bool Exists()
        {
            FileSystemInfo.Refresh();
            Debug.WriteLine($"{FullName} exists:{FileSystemInfo.Exists}");
            return FileSystemInfo.Exists;
        }

        public bool DeleteIfExists()
        {
            Repeat.Task(() => Delete())
                .WithTimeout(30.Seconds())
                .WithRetryDelay(3.Seconds())
                .Until(() => Exists() == false)
                .Now();

            return Exists() == false;
        }

        public string Name => FileSystemInfo.Name;
        public string FullName => FileSystemInfo.FullName;
        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Name);
        public string Extension => Path.GetExtension(Name);
        public DirectoryInfo Directoy => GetDir();

        public void Refresh()
        {
            FileSystemInfo.Refresh();
        }

        protected IEnumerable<string> CombineLists(IEnumerable<string> root, params string[] paths)
        {
            var allPaths = new List<string>();
            if (root != null)
                allPaths.AddRange(root);
            if (paths.Any())
                allPaths.AddRange(paths);
            return allPaths.ToArray();
        }
        protected IEnumerable<string> Combine(string root, params string[] paths)
        {
            return CombineLists(new[] { root }, paths);
        }

        public FileSystemInfo FileSystemInfo { get; }

        protected string CleanPath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return string.Empty;
            return rawPath.Trim('/').Trim('\\');
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
