using System;
using System.IO;
using System.Reflection;

namespace DotNet.Basics.IO.Robust
{
    internal class NetCoreLongPath
    {
        private static MethodInfo _normalizePath;

        private static object _win32FileSystem;
        private static MethodInfo _dirExists;
        private static MethodInfo _deleteDir;
        private static MethodInfo _createDir;

        private static MethodInfo _fileExists;
        private static MethodInfo _deleteFile;
        private static MethodInfo _moveFile;

        internal static Lazy<NetCoreLongPath> Instance = new Lazy<NetCoreLongPath>(() =>
         {
             try
             {
                 var privateCoreLib = Assembly.Load("System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
                 var pathHelper = privateCoreLib.GetType("System.IO.PathHelper");
                 _normalizePath = pathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);

                 //init internal exists
                 var systemIoFilesystem = typeof(Directory).Assembly;
                 var win32FileSystemType = systemIoFilesystem.GetType("System.IO.Win32FileSystem");
                 _win32FileSystem = Activator.CreateInstance(win32FileSystemType);

                 _dirExists = win32FileSystemType.GetMethod("DirectoryExists", BindingFlags.Public | BindingFlags.Instance);
                 _deleteDir = win32FileSystemType.GetMethod("RemoveDirectory", BindingFlags.Public | BindingFlags.Instance);
                 _createDir = win32FileSystemType.GetMethod("CreateDirectory", BindingFlags.Public | BindingFlags.Instance);

                 _fileExists = win32FileSystemType.GetMethod("FileExists", BindingFlags.Public | BindingFlags.Instance);
                 _deleteFile = win32FileSystemType.GetMethod("DeleteFile", BindingFlags.Public | BindingFlags.Instance);
                 _moveFile = win32FileSystemType.GetMethod("MoveFile", BindingFlags.Public | BindingFlags.Instance);

                 return new NetCoreLongPath();
             }
             catch (Exception e)
             {
                 throw new AggregateException("Failed to initialize long paths handling. See inner exception for details.", e);
             }
         });

        internal void CreateDir(string fullPath)
        {
            Win32System(_createDir, new object[] { fullPath });
        }

        internal void DeleteDir(string fullPath)
        {
            Win32System(_deleteDir, new object[] { fullPath, true });
        }

        internal void DeleteFile(string fullPath)
        {
            Win32System(_deleteFile, new object[] { fullPath });
        }

        internal void Movefile(string sourceFullPath, string destFullPath)
        {
            Win32System(_moveFile, new object[] { sourceFullPath, destFullPath });
        }

        internal string NormalizePath(string path)
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

        internal bool Exists(string path)
        {
            return Exists(path, _dirExists) | Exists(path, _fileExists);
        }

        private static bool Exists(string path, MethodInfo mi)
        {
            var @params = new object[] { path };
            return bool.Parse(Win32System(mi, @params).ToString());
        }

        private static object Win32System(MethodInfo mi, object[] @params)
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