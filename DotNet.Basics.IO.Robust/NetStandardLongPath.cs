using System;
using System.IO;
using System.Reflection;

namespace DotNet.Basics.IO.Robust
{
    internal class NetStandardLongPath
    {
        private static MethodInfo _normalizePath;

        private static object _win32FileSystem;
        private static MethodInfo _dirExists;
        private static MethodInfo _deleteDir;

        private static MethodInfo _fileExists;
        private static MethodInfo _deleteFile;

        internal static Lazy<NetStandardLongPath> Instance = new Lazy<NetStandardLongPath>(() =>
         {
             var privateCoreLib = Assembly.Load("System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
             var pathHelper = privateCoreLib.GetType("System.IO.PathHelper");
             string methodName = "Normalize";
             _normalizePath = pathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);

             if (_normalizePath == null)
                 throw new InvalidOperationException($"{ methodName } not found in {pathHelper.FullName}");

             //init internal exists
             var systemIoFilesystem = typeof(System.IO.Directory).Assembly;
             var win32FileSystemType = systemIoFilesystem.GetType("System.IO.Win32FileSystem");
             _win32FileSystem = Activator.CreateInstance(win32FileSystemType);

             _dirExists = win32FileSystemType.GetMethod("DirectoryExists", BindingFlags.Public | BindingFlags.Instance);
             _deleteDir = win32FileSystemType.GetMethod("RemoveDirectory", BindingFlags.Public | BindingFlags.Instance);
             _fileExists = win32FileSystemType.GetMethod("FileExists", BindingFlags.Public | BindingFlags.Instance);
             _deleteFile = win32FileSystemType.GetMethod("DeleteFile", BindingFlags.Public | BindingFlags.Instance);

             return new NetStandardLongPath();
         });

        internal bool TryDeleteDir(string fullPath)
        {
            var dirDelete = new
            {
                Mi = _deleteDir,
                Params = new object[] { fullPath, true }
            };

            try
            {
                Win32System(dirDelete.Mi, dirDelete.Params);
                return Exists(fullPath);
            }
            catch (IOException)
            {
                return false;
            }
        }

        internal bool TryDeleteFile(string fullPath)
        {
            var fileDelete = new
            {
                Mi = _deleteFile,
                Params = new object[] { fullPath }
            };

            try
            {
                Win32System(fileDelete.Mi, fileDelete.Params);
                return Exists(fullPath);
            }
            catch (IOException)
            {
                return false;
            }
        }

        internal string GetFullPath(string path)
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
