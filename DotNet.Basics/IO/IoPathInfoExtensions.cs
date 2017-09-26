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

        static IoPathInfoExtensions()
        {
            var privateCoreLib = Assembly.Load("System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            var pathHelper = privateCoreLib.GetType("System.IO.PathHelper");
            string methodName = "Normalize";
            _normalizePath = pathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);

            if (_normalizePath == null)
                throw new InvalidOperationException($"{ methodName } not found in {pathHelper.FullName}");

            //init internal exists
            //_dirInternalExists = InitInternalExists(typeof(System.IO.Directory));
            //_fileInternalExists = InitInternalExists(typeof(System.IO.File));
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
        public static bool Exists(this PathInfo pi)
        {
            return File.Exists(pi.RawPath) || Directory.Exists(pi.RawPath);
        }
    }
}
