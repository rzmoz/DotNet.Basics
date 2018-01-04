using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNet.Standard.IO
{
    public class NetCoreWin32FileSystemLongPaths : IFileSystem
    {
        private readonly object _win32FileSystem;

        //paths
        private readonly MethodInfo _pathsNormalize;
        private readonly MethodInfo _pathsEnumerate;

        //dirs
        private readonly MethodInfo _dirCreate;

        private readonly MethodInfo _dirMove;
        private readonly MethodInfo _dirExists;
        private readonly MethodInfo _dirDelete;

        //files
        private readonly MethodInfo _fileCopy;

        private readonly MethodInfo _fileMove;
        private readonly MethodInfo _fileExists;
        private readonly MethodInfo _fileDelete;


        /// <summary>
        /// NB! This will fail if you're not running it under .NET Core 2.0+
        /// </summary>
        public NetCoreWin32FileSystemLongPaths()
        {
            try
            {
                //get assemblies
                var privateCoreLib = Assembly.Load("System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
                var pathHelper = privateCoreLib.GetType("System.IO.PathHelper");

                //paths
                _pathsNormalize = pathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);

                var systemIoFilesystem = typeof(Directory).Assembly;
                var win32FileSystemType = systemIoFilesystem.GetType("System.IO.Win32FileSystem");
                _win32FileSystem = Activator.CreateInstance(win32FileSystemType);

                //paths
                _pathsEnumerate = win32FileSystemType.GetMethod("EnumeratePaths", BindingFlags.Public | BindingFlags.Instance);

                //dirs
                _dirCreate = win32FileSystemType.GetMethod("CreateDirectory", BindingFlags.Public | BindingFlags.Instance);
                _dirMove = win32FileSystemType.GetMethod("MoveDirectory", BindingFlags.Public | BindingFlags.Instance);
                _dirExists = win32FileSystemType.GetMethod("DirectoryExists", BindingFlags.Public | BindingFlags.Instance);
                _dirDelete = win32FileSystemType.GetMethod("RemoveDirectory", BindingFlags.Public | BindingFlags.Instance);

                //files
                _fileCopy = win32FileSystemType.GetMethod("CopyFile", BindingFlags.Public | BindingFlags.Instance);
                _fileMove = win32FileSystemType.GetMethod("MoveFile", BindingFlags.Public | BindingFlags.Instance);
                _fileExists = win32FileSystemType.GetMethod("FileExists", BindingFlags.Public | BindingFlags.Instance);
                _fileDelete = win32FileSystemType.GetMethod("DeleteFile", BindingFlags.Public | BindingFlags.Instance);
            }
            catch (Exception e)
            {
                throw new AggregateException("Failed to initialize long paths handling. Are you running .NET Core 2.0? See inner exception for details.", e);
            }
        }

        //paths
        public string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                var result = _pathsNormalize.Invoke(null, new object[] { path, true, true });
                return result?.ToString();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null)
                    throw;
                throw e.InnerException;
            }
        }

        //dirs
        public IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_pathsEnumerate, fullPath, searchPattern, searchOption, 3);
        }

        public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern,
            SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_pathsEnumerate, fullPath, searchPattern, searchOption, 2);
        }

        public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_pathsEnumerate, fullPath, searchPattern, searchOption, 1);
        }

        public void CreateDir(string fullPath)
        {
            Win32System(_dirCreate, fullPath);
        }

        public void MoveDir(string sourceFullPath, string destFullPath)
        {
            Win32System(_dirMove, sourceFullPath, destFullPath);
        }

        public bool ExistsDir(string fullPath)
        {
            return Exists(_dirExists, fullPath);
        }

        public void DeleteDir(string fullPath, bool recursive = true)
        {
            Win32System(_dirDelete, fullPath, recursive);
        }

        //files
        public void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            Win32System(_fileCopy, sourceFullPath, destFullPath, overwrite);
        }

        public void MoveFile(string sourceFullPath, string destFullPath)
        {
            Win32System(_fileMove, sourceFullPath, destFullPath);
        }

        public bool ExistsFile(string fullPath)
        {
            return Exists(_fileExists, fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            Win32System(_fileDelete, fullPath);
        }

        //private
        private bool Exists(MethodInfo mi, string path)
        {
            return bool.Parse(Win32System(mi, path).ToString());
        }

        private object Win32System(MethodInfo mi, params object[] @params)
        {
            try
            {
                return mi.Invoke(_win32FileSystem, @params);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null)
                    throw;
                throw e.InnerException;
            }

        }
    }
}