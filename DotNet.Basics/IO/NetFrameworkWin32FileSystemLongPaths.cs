using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO
{
    internal class NetFrameworkWin32FileSystemLongPaths : IFileSystem
    {
        //paths 
        private readonly MethodInfo _normalizePath;

        private static readonly int _maxPathLength = 32767;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;


        public NetFrameworkWin32FileSystemLongPaths()
        {
            EnsureLongPathsAreEnabled();

            //init normalize path
            var type = typeof(Path);
            string methodName = "NormalizePath";
            _normalizePath = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .OrderByDescending(m => m.GetParameters().Length)
                .FirstOrDefault();
            if (_normalizePath == null)
                throw new InvalidOperationException($"{ methodName } not found in {type.FullName}");
        }

        //paths
        public string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            var @params = new object[]
            {
                path,
                true,
                _maxPathLength,
                true
            };

            try
            {
                var result = _normalizePath.Invoke(null, @params);
                return result?.ToString();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        public IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFileSystemEntries(fullPath, searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(fullPath, searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(fullPath, searchPattern, searchOption);
        }

        public void CreateDir(string fullPath)
        {
            Directory.CreateDirectory(fullPath);
        }

        public void MoveDir(string sourceFullPath, string destFullPath)
        {
            Directory.Move(sourceFullPath, destFullPath);
        }

        public bool ExistsDir(string fullPath)
        {
            return Directory.Exists(fullPath);
        }

        public void DeleteDir(string fullPath)
        {
            Directory.Delete(fullPath, true);
        }

        public void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            File.Copy(sourceFullPath, destFullPath, overwrite);
        }

        public void MoveFile(string sourceFullPath, string destFullPath)
        {
            File.Move(sourceFullPath, destFullPath);
        }

        public bool ExistsFile(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            File.Delete(fullPath);
        }

        private static void EnsureLongPathsAreEnabled()
        {
            var type = typeof(Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxDirectoryLength);
        }
    }
}
