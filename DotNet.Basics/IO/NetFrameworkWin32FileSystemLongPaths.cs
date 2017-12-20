using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class NetFrameworkWin32FileSystemLongPaths : IFileSystem
    {
        private const uint _maxLongPathLength = 32767;

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
            var path = typeof(Path);
            var mscorlib = path.Assembly;

            //enable long paths
            AppContext.SetSwitch("Switch.System.IO.BlockLongPaths", false);
            var appContextSwitches = mscorlib.GetType("System.AppContextSwitches");
            appContextSwitches.GetField("_blockLongPaths", BindingFlags.Static | BindingFlags.NonPublic).SetValue("_blockLongPaths", -1);
            appContextSwitches.GetField("_useLegacyPathHandling", BindingFlags.Static | BindingFlags.NonPublic).SetValue("_useLegacyPathHandling", -1);

            var longPathHelper = mscorlib.GetType("System.IO.LongPathHelper");
            _pathsNormalize = longPathHelper.GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Static);

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

            return StaticInvoke(_pathsNormalize, path, _maxLongPathLength, false, true)?.ToString();
        }

        public IEnumerable<string> EnumeratePaths(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFileSystemEntries(fullPath.EnsurePrefix(Paths.ExtendedPathPrefix), searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(fullPath.EnsurePrefix(Paths.ExtendedPathPrefix), searchPattern, searchOption);
        }

        public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(fullPath.EnsurePrefix(Paths.ExtendedPathPrefix), searchPattern, searchOption);
        }

        public void CreateDir(string fullPath)
        {
            StaticInvoke(_dirCreate, fullPath);
        }

        public void MoveDir(string sourceFullPath, string destFullPath)
        {
            StaticInvoke(_dirMove, sourceFullPath, destFullPath);
        }

        public bool ExistsDir(string fullPath)
        {
            return (bool)StaticInvoke(_dirExists, fullPath);
        }

        public void DeleteDir(string fullPath, bool recursive = true)
        {
            StaticInvoke(_dirDelete, fullPath, recursive);
        }

        public void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            StaticInvoke(_fileCopy, sourceFullPath, destFullPath, overwrite);
        }

        public void MoveFile(string sourceFullPath, string destFullPath)
        {
            StaticInvoke(_fileMove, sourceFullPath, destFullPath);
        }

        public bool ExistsFile(string fullPath)
        {
            return (bool)StaticInvoke(_fileExists, fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            StaticInvoke(_fileDelete, fullPath);
        }

        private object StaticInvoke(MethodInfo mi, params object[] parameters)
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
    }
}
