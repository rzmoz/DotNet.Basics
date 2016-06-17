using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Boolean;

namespace DotNet.Basics.IO
{
    internal static class SystemIoPath
    {
        private static readonly MethodInfo _normalizePath;
        private static readonly MethodInfo _dirInternalExists;
        private static readonly MethodInfo _fileInternalExists;

        private static readonly int _maxPathLength = 32000;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;

        static SystemIoPath()
        {
            EnsureLongPathsAreEnabled();

            //init normalize path
            var type = typeof(System.IO.Path);
            string methodName = "NormalizePath";
            _normalizePath = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                        .Where(m => m.Name == methodName)
                                        .OrderByDescending(m => m.GetParameters().Length)
                                        .FirstOrDefault();
            if (_normalizePath == null)
                throw new InvalidOperationException($"{methodName } not found in {type.FullName}");

            //init internal exists
            _dirInternalExists = InitInternalExists(typeof(System.IO.Directory));
            _fileInternalExists = InitInternalExists(typeof(System.IO.File));
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

        public static string GetFullPath(string path)
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
                if (e.InnerException is System.IO.PathTooLongException)
                    throw new System.IO.PathTooLongException($"The specified path, file name, or both are too long. The fully qualified file name must be less than {_maxPathLength} characters, and the directory name must be less than {_maxDirectoryLength} characters");

                throw e.InnerException;
            }
        }

        public static bool Exists(this Path path)
        {
            return Exists(path.FullName, path.IsFolder);
        }

        public static bool Exists(string path, bool isFolder)
        {
            if (path == null)
                return false;

            var method = isFolder ? _dirInternalExists : _fileInternalExists;

            var @params = new object[]
            {
                path
            };

            try
            {
                var result = method.Invoke(null, @params);
                return Parse(result.ToString());
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }


        private static void EnsureLongPathsAreEnabled()
        {
            var type = typeof(System.IO.Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxPathLength);
        }

        private static IEnumerable<MethodInfo> GetMethods(string methodName)
        {
            var type = typeof(System.IO.Path);

            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName);
        }
    }
}
