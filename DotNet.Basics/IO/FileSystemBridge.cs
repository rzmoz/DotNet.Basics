using System.Collections.Generic;
using System.IO;

namespace DotNet.Basics.IO
{
    public class FileSystemBridge : IFileSystem
    {
        //paths
        public virtual string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return Path.GetFullPath(path);
        }

        //dirs
        public virtual IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFileSystemEntries(fullPath, searchPattern, searchOption);
        }
        public virtual IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(fullPath, searchPattern, searchOption);
        }
        public virtual IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(fullPath, searchPattern, searchOption);
        }

        public virtual void CreateDir(string fullPath)
        {
            Directory.CreateDirectory(fullPath);
        }
        public virtual void MoveDir(string sourceFullPath, string destFullPath)
        {
            Directory.Move(sourceFullPath, destFullPath);
        }
        public virtual bool ExistsDir(string fullPath)
        {
            return Directory.Exists(fullPath);
        }
        public virtual void DeleteDir(string fullPath, bool recursive = true)
        {
            Directory.Delete(fullPath, recursive);
        }

        //files
        public virtual void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            File.Copy(sourceFullPath, destFullPath, overwrite);
        }
        public virtual void MoveFile(string sourceFullPath, string destFullPath)
        {
            File.Move(sourceFullPath, destFullPath);
        }
        public virtual bool ExistsFile(string fullPath)
        {
            return File.Exists(fullPath);
        }
        public virtual void DeleteFile(string fullPath)
        {
            File.Delete(fullPath);
        }
    }
}