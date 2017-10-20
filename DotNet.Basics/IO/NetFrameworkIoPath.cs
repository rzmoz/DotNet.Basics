using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO
{
    internal static class NetFrameworkIoPath
    {
        private static readonly MethodInfo _normalizePath;
        private static readonly MethodInfo _dirInternalExists;
        private static readonly MethodInfo _fileInternalExists;

        private static readonly int _maxPathLength = 32000;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;

        static NetFrameworkIoPath()
        {
#if NET47
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
#endif
        }

        public static void TryDeleteDir(string fullPath, bool isFolder)
        {
            try
            {
                Directory.Delete(fullPath, true);
            }
            catch (IOException)
            { }
        }

        public static void TryDeleteFile(string fullPath, bool isFolder)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (IOException)
            { }
        }

        internal static string GetFullPath(string path)
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
                if (e.InnerException == null)
                    throw;

                if (e.InnerException is System.IO.PathTooLongException)
                    throw new System.IO.PathTooLongException($"The specified path, file name, or both are too long. The fully qualified file name must be less than {_maxPathLength} characters, and the directory name must be less than {_maxDirectoryLength} characters");

                //we don't support uris so it's just returned as is
                if (e.InnerException is ArgumentException && e.InnerException.Message.Equals("URI formats are not supported.", StringComparison.OrdinalIgnoreCase))
                    return path;

                throw e.InnerException;
            }
        }

        internal static bool Exists(string path, bool isFolder, IfNotExists ifNotExists = IfNotExists.Mute)
        {
            bool found;

            if (path == null)
                found = false;
            else
            {
                var method = isFolder ? _dirInternalExists : _fileInternalExists;

                var @params = new object[]
                {
                path
                };

                try
                {
                    var result = method.Invoke(null, @params);
                    found = bool.Parse(result.ToString());
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException == null)
                        throw;
                    throw e.InnerException;
                }
            }

            if (found == false && ifNotExists == IfNotExists.ThrowIoException)
                throw new IOException($"{path} not found");
            return found;
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
            var type = typeof(System.IO.Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxPathLength);
        }
    }
}
