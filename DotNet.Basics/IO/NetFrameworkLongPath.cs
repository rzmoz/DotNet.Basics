using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO
{
    internal class NetFrameworkWin32LongPath : IFileSystem
    {
        //paths 
        private readonly MethodInfo _normalizePath;
        //dirs
        private readonly MethodInfo _dirInternalExists;

        //files
        private readonly MethodInfo _fileInternalExists;
        private static readonly int _maxPathLength = 32767;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;


        public NetFrameworkWin32LongPath()
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

            //init internal exists
            _dirInternalExists = InitInternalExists(typeof(System.IO.Directory));
            _fileInternalExists = InitInternalExists(typeof(System.IO.File));

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
            throw new NotImplementedException();
        }

        public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public void CreateDir(string fullPath)
        {
            throw new NotImplementedException();
        }

        public void MoveDir(string sourceFullPath, string destFullPath)
        {
            throw new NotImplementedException();
        }

        public bool ExistsDir(string fullPath)
        {
            return Exists(_dirInternalExists, fullPath);
        }

        public void DeleteDir(string fullPath)
        {
            throw new NotImplementedException();
        }

        public void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string sourceFullPath, string destFullPath)
        {
            throw new NotImplementedException();
        }

        public bool ExistsFile(string fullPath)
        {
            return Exists(_fileInternalExists, fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            throw new NotImplementedException();
        }

        //private
        private static bool Exists(MethodInfo mi, params object[] @params)
        {
            try
            {
                var result = mi.Invoke(null, @params);
                return bool.Parse(result.ToString());
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null)
                    throw;
                throw e.InnerException;
            }
        }

        private static MethodInfo InitInternalExists(Type type)
        {
            var methodName = "InternalExists";

            var method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .OrderBy(m => m.GetParameters().Length)
                .FirstOrDefault();

            if (method == null)
                throw new InvalidOperationException($"{methodName } not found in {type.FullName}");

            return method;
        }

        private static void EnsureLongPathsAreEnabled()
        {
            var type = typeof(Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxDirectoryLength);
        }
    }
}
