using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNet.Basics.IO.Robust
{
    public class NetCoreLongPath
    {
        private static readonly MethodInfo _enumeratePaths;
        private static readonly MethodInfo _normalizePath;

        private static readonly object _win32FileSystem;
        private static readonly MethodInfo _dirExists;
        private static readonly MethodInfo _deleteDir;
        private static readonly MethodInfo _createDir;

        private static readonly MethodInfo _fileExists;
        private static readonly MethodInfo _deleteFile;
        private static readonly MethodInfo _moveFile;
        private static readonly MethodInfo _copyFile;

        static NetCoreLongPath()
        {
            try
            {
                var privateCoreLib = Assembly.Load("System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
                var pathHelper = privateCoreLib.GetType("System.IO.PathHelper");
                _normalizePath = pathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);


                var systemIoFilesystem = typeof(Directory).Assembly;
                var win32FileSystemType = systemIoFilesystem.GetType("System.IO.Win32FileSystem");
                _win32FileSystem = Activator.CreateInstance(win32FileSystemType);

                _enumeratePaths = win32FileSystemType.GetMethod("EnumeratePaths", BindingFlags.Public | BindingFlags.Instance);

                _dirExists = win32FileSystemType.GetMethod("DirectoryExists", BindingFlags.Public | BindingFlags.Instance);
                _deleteDir = win32FileSystemType.GetMethod("RemoveDirectory", BindingFlags.Public | BindingFlags.Instance);
                _createDir = win32FileSystemType.GetMethod("CreateDirectory", BindingFlags.Public | BindingFlags.Instance);

                _fileExists = win32FileSystemType.GetMethod("FileExists", BindingFlags.Public | BindingFlags.Instance);
                _deleteFile = win32FileSystemType.GetMethod("DeleteFile", BindingFlags.Public | BindingFlags.Instance);
                _moveFile = win32FileSystemType.GetMethod("MoveFile", BindingFlags.Public | BindingFlags.Instance);
                _copyFile = win32FileSystemType.GetMethod("CopyFile", BindingFlags.Public | BindingFlags.Instance);

            }
            catch (Exception e)
            {
                throw new AggregateException("Failed to initialize long paths handling. See inner exception for details.", e);
            }
        }

        //dirs
        public static void CreateDir(string fullPath)
        {
            Win32System(_createDir, fullPath);
        }

        public static void DeleteDir(string fullPath)
        {
            Win32System(_deleteDir, fullPath, true);
        }

        //files
        public static void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            Win32System(_copyFile, sourceFullPath, destFullPath, overwrite);
        }
        public static void DeleteFile(string fullPath)
        {
            Win32System(_deleteFile, fullPath);
        }
        public static void MoveFile(string sourceFullPath, string destFullPath)
        {
            Win32System(_moveFile, sourceFullPath, destFullPath);
        }

        //paths
        public static IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_enumeratePaths, fullPath, searchPattern, searchOption, 3);
        }
        public static IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_enumeratePaths, fullPath, searchPattern, searchOption, 2);
        }
        public static IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return (IEnumerable<string>)Win32System(_enumeratePaths, fullPath, searchPattern, searchOption, 1);
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var @params = new object[] { path, true, true };

            try
            {
                var result = _normalizePath.Invoke(null, @params);
                return result?.ToString();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null)
                    throw;
                throw e.InnerException;
            }
        }

        public static bool Exists(string path)
        {
            return Exists(path, _dirExists) | Exists(path, _fileExists);
        }

        private static bool Exists(string path, MethodInfo mi)
        {
            return bool.Parse(Win32System(mi, path).ToString());
        }

        private static object Win32System(MethodInfo mi, params object[] @params)
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