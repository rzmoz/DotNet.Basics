using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.IO
{
    public class NetFrameworkWin32FileSystemLongPaths : IFileSystem
    {
        private static readonly int _maxPathLength = 32767;
        private static readonly int _maxDirectoryLength = _maxPathLength - 12;

        //paths
        private readonly MethodInfo _pathsNormalize;

        //dirs
        private readonly MethodInfo _dirCreate;
        private readonly MethodInfo _dirMove;
        private readonly MethodInfo _dirExists;
        private readonly MethodInfo _dirDelete;

        //files
        private readonly MethodInfo _fileCopy;
        private readonly MethodInfo _fileMove;
        private readonly MethodInfo _fileExists;
        private readonly MethodInfo _fileDelete;

        public NetFrameworkWin32FileSystemLongPaths()
        {
            EnsureLongPathsAreEnabled();

            var mscorlib = typeof(Path).Assembly;

            //init normalize path
            var longPath = mscorlib.GetType("System.IO.LongPath");
            _pathsNormalize = longPath.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == "NormalizePath").OrderBy(m => m.GetParameters().Length).FirstOrDefault();

            var longPathDirectory = mscorlib.GetType("System.IO.LongPathDirectory");
            _dirCreate = longPathDirectory.GetMethod("CreateDirectory", BindingFlags.NonPublic | BindingFlags.Static);
            _dirMove = longPathDirectory.GetMethod("Move", BindingFlags.NonPublic | BindingFlags.Static);
            _dirExists = longPathDirectory.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == "InternalExists").OrderBy(m => m.GetParameters().Length).FirstOrDefault();
            _dirDelete = longPathDirectory.GetMethod("Delete", BindingFlags.NonPublic | BindingFlags.Static);

            var longPathFile = mscorlib.GetType("System.IO.LongPathFile");
            _fileCopy = longPathFile.GetMethod("Copy", BindingFlags.NonPublic | BindingFlags.Static);
            _fileMove = longPathFile.GetMethod("Move", BindingFlags.NonPublic | BindingFlags.Static);
            _fileExists = longPathFile.GetMethod("InternalExists", BindingFlags.NonPublic | BindingFlags.Static);
            _fileDelete = longPathFile.GetMethod("Delete", BindingFlags.NonPublic | BindingFlags.Static);
        }

        //paths
        public string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return StaticInvoke(_pathsNormalize, new object[] { path })?.ToString();
        }

        //dirs
        public IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFileSystemEntries(fullPath, searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(fullPath, searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(fullPath, searchPattern, searchOption);
        }

        public void CreateDir(string fullPath)
        {
            StaticInvoke(_dirCreate, new object[] { fullPath });
        }

        public void MoveDir(string sourceFullPath, string destFullPath)
        {
            StaticInvoke(_dirMove, new object[] { sourceFullPath, destFullPath });
        }

        public bool ExistsDir(string fullPath)
        {
            return (bool)StaticInvoke(_dirExists, new object[] { fullPath });
        }

        public void DeleteDir(string fullPath, bool recursive = true)
        {
            StaticInvoke(_dirDelete, new object[] { fullPath, recursive });
        }

        //files
        public void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            StaticInvoke(_fileCopy, new object[] { sourceFullPath, destFullPath, overwrite });
        }

        public void MoveFile(string sourceFullPath, string destFullPath)
        {
            StaticInvoke(_fileMove, new object[] { sourceFullPath, destFullPath });
        }

        public bool ExistsFile(string fullPath)
        {
            return (bool)StaticInvoke(_fileExists, new object[] { fullPath });
        }

        public void DeleteFile(string fullPath)
        {
            StaticInvoke(_fileDelete, new object[] { fullPath });
        }

        //private
        private object StaticInvoke(MethodInfo mi, object[] parameters)
        {
            try
            {
                return mi.Invoke(null, parameters);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }
        private static void EnsureLongPathsAreEnabled()
        {
            var type = typeof(Path);
            type?.GetField("MaxPath", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxPath", _maxPathLength);
            type?.GetField("MaxDirectoryLength", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue("MaxDirectoryLength", _maxDirectoryLength);
        }
    }
}
