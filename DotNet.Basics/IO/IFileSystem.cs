using System.Collections.Generic;
using System.IO;

namespace DotNet.Basics.IO
{
    public interface IFileSystem
    {
        //paths
        string GetFullPath(string path);
        IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption);
        IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption);
        IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption);

        //dirs
        void CreateDir(string fullPath);
        void MoveDir(string sourceFullPath, string destFullPath);
        bool ExistsDir(string fullPath);
        void DeleteDir(string fullPath);

        //files
        void CopyFile(string sourceFullPath, string destFullPath, bool overwrite);
        void MoveFile(string sourceFullPath, string destFullPath);
        bool ExistsFile(string fullPath);
        void DeleteFile(string fullPath);
    }
}
