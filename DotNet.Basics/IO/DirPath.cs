using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class DirPath : Path
    {
        public DirPath(string fullPath)
            : this(new[] { fullPath })
        { }
        public DirPath(string[] pathSegments)
            : this(pathSegments, PathDelimiter.Backslash)
        { }
        public DirPath(string[] pathSegments, PathDelimiter delimiter)
            : base(pathSegments, true, delimiter)
        { }

        /// <summary>
        /// Returns a new Path where original and added paths are combined
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public new DirPath Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new DirPath(combinedSegments, Delimiter);
        }

        public DirPath[] GetDirectories(string searchPattern = null)
        {
            var subDirs = GetChildItems(new[] {new KeyValuePair<string, object>("Dir", null)},searchPattern);
            return subDirs.Select(dir => dir.ToDir()).ToArray();
        }
        public FilePath[] GetFiles(string searchPattern = null)
        {
            var subDirs = GetChildItems(new[] { new KeyValuePair<string, object>("File", null) }, searchPattern);
            return subDirs.Select(dir => dir.ToFile()).ToArray();
        }
        public Path[] GetPaths(string searchPattern = null)
        {
            return ToDirectoryInfo().GetFileSystemInfos(searchPattern ?? "*").Select<System.IO.FileSystemInfo, Path>(fsi =>
             {
                 if (fsi is System.IO.DirectoryInfo)
                     return fsi.FullName.ToDir();
                 return fsi.FullName.ToFile();
             }).ToArray();
        }

        private string[] GetChildItems(KeyValuePair<string, object>[] parameters, string searchPattern = null)
        {
            var cmdlet = new PowerShellCmdlet("Get-ChildItem")
                .AddParameter("Path", FullName);

            foreach (var parameter in parameters)
                cmdlet.AddParameter(parameter.Key, parameter.Value);

            if (searchPattern != null)
                cmdlet.AddParameter("Include", searchPattern);

            var result = PowerShellConsole.RunScript(cmdlet.ToScript());

            return result.Select(dir => (string)((dynamic)dir).FullName.ToString()).ToArray();
        }

        public IEnumerable<DirPath> EnumerateDirectories(string searchPattern = null)
        {
            return ToDirectoryInfo().EnumerateDirectories(searchPattern ?? "*").Select(dir => dir.FullName.ToDir());
        }
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern = null)
        {
            return ToDirectoryInfo().EnumerateFiles(searchPattern ?? "*").Select(file => file.FullName.ToFile());
        }
        public IEnumerable<Path> EnumeratePaths(string searchPattern = null)
        {
            return ToDirectoryInfo().EnumerateFileSystemInfos(searchPattern ?? "*").Select<System.IO.FileSystemInfo, Path>(fsi =>
            {
                if (fsi is System.IO.DirectoryInfo)
                    return fsi.FullName.ToDir();
                return fsi.FullName.ToFile();
            });
        }

        public DirectorySecurity GetAccessControl()
        {
            return ToDirectoryInfo().GetAccessControl();
        }
        public void SetAccessControl(DirectorySecurity directorySecurity)
        {
            ToDirectoryInfo().SetAccessControl(directorySecurity);
        }
        public System.IO.DirectoryInfo ToDirectoryInfo()
        {
            return new System.IO.DirectoryInfo(RawName);
        }
    }
}
