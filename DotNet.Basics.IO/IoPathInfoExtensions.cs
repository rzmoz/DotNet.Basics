using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public static class IoPathInfoExtensions
    {
        private static readonly MethodInfo _normalizePath;

        private static readonly object _win32FileSystem;
        private static readonly MethodInfo _dirExists;
        private static readonly MethodInfo _deleteDir;

        private static readonly MethodInfo _fileExists;
        private static readonly MethodInfo _deleteFile;

        static IoPathInfoExtensions()
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
        }

        public static DirPath Directory(this PathInfo pi)
        {
            return pi.IsFolder ? pi.ToDir() : pi.Parent;
        }

        public static bool DeleteIfExists(this PathInfo pi)
        {
            return pi.DeleteIfExists(30.Seconds());
        }

        public static bool DeleteIfExists(this PathInfo pi, TimeSpan timeout)
        {
            if (pi.Exists() == false)
                return true;

            var dirDelete = new
            {
                Mi = _deleteDir,
                Params = new object[] { pi.FullPath(), true }
            };
            var fileDelete = new
            {
                Mi = _deleteFile,
                Params = new object[] { pi.FullPath() }
            };
            Repeat.Task(() =>
                {
                    try
                    {
                        Win32System(dirDelete.Mi, dirDelete.Params);
                    }
                    catch (IOException )
                    { }
                    try
                    {
                        Win32System(fileDelete.Mi, fileDelete.Params);
                    }
                    catch (IOException)
                    { }
                })
                .WithOptions(o =>
                {
                    o.Timeout = timeout;
                    o.RetryDelay = 2.Seconds();
                    o.DontRethrowOnTaskFailedType = typeof(IOException);
                })
                .Until(() => pi.Exists() == false);

            return pi.Exists() == false;
        }

        public static string FullPath(this PathInfo pi)
        {
            return GetFullPath(pi.RawPath);
        }
        public static string GetFullPath(string path)
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
                throw e.InnerException;
            }
        }

        public static bool Exists(this PathInfo pi, bool throwIoExceptionIfNotExists = false)
        {
            var fullPath = pi.FullPath();
            return fullPath.Exists(_dirExists) | fullPath.Exists(_fileExists);
        }
        private static bool Exists(this string fullPath, MethodInfo mi, bool throwIoExceptionIfNotExists = false)
        {
            var @params = new object[] { fullPath };
            var found = bool.Parse(Win32System(mi, @params).ToString());
            if (found == false && throwIoExceptionIfNotExists)
                throw new IOException($"{fullPath} not found");
            return found;
        }

        private static object Win32System(MethodInfo mi, object[] @params)
        {
            try
            {
                return mi.Invoke(_win32FileSystem, @params);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}