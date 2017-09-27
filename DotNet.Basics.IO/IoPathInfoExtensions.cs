using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoPathInfoExtensions
    {
        private static readonly MethodInfo _normalizePath;

        private static readonly object _win32FileSystem;
        private static readonly MethodInfo _dirExists;
        private static readonly MethodInfo _fileExists;

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
            _fileExists = win32FileSystemType.GetMethod("FileExists", BindingFlags.Public | BindingFlags.Instance);
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
            return pi.FullPath().Exists(pi.IsFolder ? _dirExists : _fileExists, throwIoExceptionIfNotExists);
        }
        private static bool Exists(this string fullPath, MethodInfo mi, bool throwIoExceptionIfNotExists = false)
        {
            bool found;

            if (fullPath == null)
                found = false;
            else
            {
                var @params = new object[] { fullPath };

                try
                {
                    var result = mi.Invoke(_win32FileSystem, @params);
                    found = bool.Parse(result.ToString());
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }

            if (found == false && throwIoExceptionIfNotExists)
                throw new IOException($"{fullPath} not found");
            return found;
        }
    }
}
