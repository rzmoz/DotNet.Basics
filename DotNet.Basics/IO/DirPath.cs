using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

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

        public new DirPath Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new DirPath(combinedSegments, Delimiter);
        }

        public DirPath[] GetDirectories(string searchPattern = null)
        {
            return ToDirectoryInfo().GetDirectories(searchPattern ?? "*").Select(dir => dir.FullName.ToDir()).ToArray();
        }
        public FilePath[] GetFiles(string searchPattern = null)
        {
            return ToDirectoryInfo().GetFiles(searchPattern ?? "*").Select(file => file.FullName.ToFile()).ToArray();
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
