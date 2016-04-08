using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public abstract class IoPath<T> where T : FileSystemInfo
    {
        private readonly FileSystemInfoFactory _fileSystemInfoFactory;
        private readonly List<string> _paths;

        private IoPath(IEnumerable<string> paths)
        {
            _fileSystemInfoFactory = new FileSystemInfoFactory();
            _paths = paths.Select(CleanPath).ToList();
            SetFileSystemInfo();
        }

        protected IoPath(string root, params string[] paths)
            : this(Combine(root, paths))
        {

        }
        protected IoPath(FileSystemInfo root, params string[] paths)
            : this(root.FullName, paths)
        {
        }

        protected IoPath(IoDir root, params string[] paths)
            : this(CombineLists(root._paths, paths))
        {
        }
        protected IoPath(IoPath<T> root, params string[] paths)
            : this(CombineLists(root._paths, paths))
        {
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


        private static IEnumerable<string> CombineLists(IEnumerable<string> root, params string[] paths)
        {
            var allPaths = new List<string>();
            if (root != null)
                allPaths.AddRange(root);
            if (paths.Any())
                allPaths.AddRange(paths);
            return allPaths.ToArray();
        }
        private static IEnumerable<string> Combine(string root, params string[] paths)
        {
            return CombineLists(new[] { root }, paths);
        }

        public T FileSystemInfo { get; private set; }


        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string NameWithoutExtension { get; private set; }
        public string Extension { get; private set; }

        public void Refresh()
        {
            FileSystemInfo.Refresh();
        }

        private void SetFileSystemInfo()
        {
            var fullPath = Path.Combine(_paths.ToArray());

            FileSystemInfo = _fileSystemInfoFactory.Create<T>(fullPath);
            Name = FileSystemInfo.Name;
            FullName = FileSystemInfo.FullName;

            var extensionIndex = Name.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            if (extensionIndex < 0)
            {
                NameWithoutExtension = Name;
                Extension = string.Empty;
            }
            else
            {
                NameWithoutExtension = Name.Substring(0, extensionIndex);
                Extension = Name.Substring(extensionIndex);
            }
        }

        private static string CleanPath(string rawPath)
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
