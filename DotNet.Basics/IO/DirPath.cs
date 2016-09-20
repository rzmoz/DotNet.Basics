using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
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

        public void CleanIfExists()
        {
            if (IsFolder == false)
                throw new PathException($"Can't clean path because it's not a folder", this);
            try
            {
                PowerShellConsole.RemoveItem($"{FullName}\\*", force: true, recurse: true);
            }
            catch (ItemNotFoundException)
            {
            }
        }

        public void CreateIfNotExists()
        {
            if (this.Exists())
                return;

            if (IsFolder == false)
                throw new PathException($"Can't create path because it's not a folder {FullName}", this);

            try
            {
                System.IO.Directory.CreateDirectory(FullName);
            }
            catch (System.IO.IOException e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

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

        public DirPath[] GetDirectories(string searchPattern = null, bool recurse = false)
        {
            var dirs = PowerShellConsole.GetChildItem(FullName, recurse, searchPattern, "Dir");
            return dirs.Select(dir => dir.ToDir()).ToArray();
        }
        public FilePath[] GetFiles(string searchPattern = null, bool recurse = false)
        {
            var dirs = PowerShellConsole.GetChildItem(FullName, recurse, searchPattern, "File");
            return dirs.Select(dir => dir.ToFile()).ToArray();
        }
        public Path[] GetPaths(string searchPattern = null, bool recurse = false)
        {
            var dirs = PowerShellConsole.GetChildItem(FullName, recurse, searchPattern);
            return dirs.Select(dir => dir.ToPath()).ToArray();
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
