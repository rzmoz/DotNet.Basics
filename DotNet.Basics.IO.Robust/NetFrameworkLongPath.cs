using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO.Robust
{/*
    internal class NetFrameworkLongPath
    {
        private static MethodInfo _normalizePath;
        private static MethodInfo _dirInternalExists;
        private static MethodInfo _fileInternalExists;

        private static int _maxPathLength = 32767;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;

        //lazy to ensure it's only initialized on purpose to avoid exceptions when loaded in netstanrd env
        internal static Lazy<NetFrameworkLongPath> Instance => new Lazy<NetFrameworkLongPath>(() =>
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
                 throw new InvalidOperationException($"{ methodName } not found in {type.FullName}");

             //init internal exists
             _dirInternalExists = InitInternalExists(typeof(System.IO.Directory));
             _fileInternalExists = InitInternalExists(typeof(System.IO.File));

             return new NetFrameworkLongPath();
         });

        internal bool TryCreateDir(string fullPath)
        {
            throw new NotImplementedException();
        }

        internal bool TryDeleteDir(string fullPath)
        {
            try
            {
                Directory.Delete(fullPath);
                return Exists(fullPath);
            }
            catch (IOException)
            {
                return false;
            }
        }

        internal bool TryDeleteFile(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
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
                switch (e.InnerException)
                {
                    case null:
                        throw;
                    case PathTooLongException _:
                        throw new System.IO.PathTooLongException($"The specified path, file name, or both are too long. The fully qualified file name must be less than {_maxPathLength} characters, and the directory name must be less than {_maxDirectoryLength} characters");
                    case ArgumentException _ when e.InnerException.Message.Equals("URI formats are not supported.", StringComparison.OrdinalIgnoreCase):
                        return path;
                }

                //we don't support uris so it's just returned as is
                throw e.InnerException;
            }
        }

        public bool Exists(string path)
        {
            var @params = new object[] { path };

            return Exists(_dirInternalExists, @params) || Exists(_fileInternalExists, @params);
        }

        private static bool Exists(MethodInfo mi, object[] @params)
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
            var type = typeof(System.IO.Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxPathLength);
        }
    }*/
}
