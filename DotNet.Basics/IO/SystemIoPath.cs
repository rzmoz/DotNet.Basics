using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO
{
    internal static class SystemIoPath
    {
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            EnsureLongPathsAreEnabled();

            var type = typeof(System.IO.Path);

            string methodName = "NormalizePath";

            var method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .OrderByDescending(m => m.GetParameters().Length)
                .FirstOrDefault();

            if (method == null)
                throw new InvalidOperationException($"{methodName } not found in {type.FullName}");

            var @params = new object[]
            {
                path,
                true,
                32000,
                true
            };

            var result = method.Invoke(null, @params);
            return result?.ToString();
        }

        public static bool Exists(Path path)
        {
            if (path == null)
                return false;

            EnsureLongPathsAreEnabled();

            var type = path.IsFolder ? typeof(System.IO.Directory) : typeof(System.IO.File);

            string methodName = "InternalExists";

            var method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .OrderBy(m => m.GetParameters().Length)
                .FirstOrDefault();

            if (method == null)
                throw new InvalidOperationException($"{methodName } not found in {type.FullName}");

            var @params = new object[]
            {
                path.FullName
            };

            var result = method.Invoke(null, @params);
            return Boolean.Parse(result.ToString());
        }

        private static void EnsureLongPathsAreEnabled()
        {
            var type = typeof(System.IO.Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", 3200);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", 3200);
        }

        private static IEnumerable<MethodInfo> GetMethods(string methodName)
        {
            var type = typeof(System.IO.Path);

            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == methodName);
        }
    }
}
