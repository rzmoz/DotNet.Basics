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
        protected IoPath(FileSystemInfo fsi)
        {
            if (fsi == null) throw new ArgumentNullException(nameof(fsi));
            FileSystemInfo = fsi;
        }

        public bool Exists()
        {
            FileSystemInfo.Refresh();
            var exists = FileSystemInfo.Exists;
            Debug.WriteLine("{0} exists:{1}", FullName, exists);
            return exists;
        }

        public bool DeleteIfExists()
        {

            Repeat.Task(() =>
            {
                if (FileSystemInfo is DirectoryInfo)
                    Directory.Delete(FileSystemInfo.FullName, true);
                else if (FileSystemInfo is FileInfo)
                    File.Delete(FileSystemInfo.FullName);

                FileSystemInfo.Refresh();
            })
                .WithTimeout(1.Minutes())
                .WithRetryDelay(1.Seconds())
                .Until(() => FileSystemInfo.Exists == false)
                .Now();

            return FileSystemInfo.Exists == false;
        }

        public string Name => FileSystemInfo.Name;
        public string FullName => FileSystemInfo.FullName;
        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Name);
        public string Extension => Path.GetExtension(Name);

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
