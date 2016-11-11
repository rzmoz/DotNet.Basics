using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.AccessControl;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class DirPath : PathInfo
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
            { }
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
                DebugOut.WriteLine(e.ToString());
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
            return System.IO.Directory.GetDirectories(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir()).ToArray();
        }
        public FilePath[] GetFiles(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFiles(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToFile()).ToArray();
        }
        public PathInfo[] GetPaths(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.GetFileSystemEntries(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToPath()).ToArray();
        }
        public IEnumerable<DirPath> EnumerateDirectories(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateDirectories(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(dir => dir.ToDir());
        }
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFiles(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(file => file.ToFile());
        }
        public IEnumerable<PathInfo> EnumeratePaths(string searchPattern = null, bool recurse = false)
        {
            return System.IO.Directory.EnumerateFileSystemEntries(FullName, searchPattern ?? "*", ToSearchOption(recurse)).Select(fse => fse.ToPath());
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

        private SearchOption ToSearchOption(bool recurse)
        {
            return recurse ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;
        }
    }
}
