using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public abstract class PathInfo
    {
        private static readonly MethodInfo _normalizePath;

        private static readonly object _win32FileSystem;
        private static readonly MethodInfo _dirExists;
        private static readonly MethodInfo _deleteDir;

        private static readonly MethodInfo _fileExists;
        private static readonly MethodInfo _deleteFile;


        private static readonly char[] _separatorDetectors = { PathSeparator.Backslash, PathSeparator.Slash };

        static PathInfo()
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

        protected PathInfo(string path, params string[] segments)
            : this(path, IO.IsFolder.Unknown, segments)
        { }

        protected PathInfo(string path, IsFolder isFolder, params string[] segments)
            : this(path, isFolder, PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, IsFolder isFolder, char pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = isFolder == IO.IsFolder.Unknown ? DetectIsFolder(path, segments) : isFolder == IO.IsFolder.True;

            Separator = DetectPathSeparator(pathSeparator, combinedSegments);

            //Clean segments
            Segments = CleanSegments(combinedSegments, Separator).ToArray();

            //Set rawpath
            RawPath = string.Join(Separator.ToString(), Segments);
            RawPath = IsFolder ? RawPath.EnsureSuffix(Separator) : RawPath.RemoveSuffix(Separator);

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Separator));
        }

        public string RawPath { get; }
        public string Name { get; }
        public bool IsFolder { get; }

        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        public char Separator { get; }
        public IReadOnlyCollection<string> Segments;

        public DirPath Directory()
        {
            return IsFolder ? this.ToDir() : this.Parent;
        }

        public bool DeleteIfExists()
        {
            return DeleteIfExists(30.Seconds());
        }

        public bool DeleteIfExists(TimeSpan timeout)
        {
            if (Exists() == false)
                return true;

            var dirDelete = new
            {
                Mi = _deleteDir,
                Params = new object[] { FullPath(), true }
            };
            var fileDelete = new
            {
                Mi = _deleteFile,
                Params = new object[] { FullPath() }
            };
            Repeat.Task(() =>
            {
                try
                {
                    Win32System(dirDelete.Mi, dirDelete.Params);
                }
                catch (IOException)
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
                .Until(() => Exists() == false);

            return Exists() == false;
        }

        public string FullPath()
        {
            return GetFullPath(RawPath);
        }
        private string GetFullPath(string path)
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

        public bool Exists(bool throwIoExceptionIfNotExists = false)
        {
            return Exists(_dirExists, throwIoExceptionIfNotExists) | Exists(_fileExists, throwIoExceptionIfNotExists);
        }

        private bool Exists(MethodInfo mi, bool throwIoExceptionIfNotExists = false)
        {
            var @params = new object[] { RawPath };
            var found = bool.Parse(Win32System(mi, @params).ToString());
            if (found == false && throwIoExceptionIfNotExists)
                throw new IOException($"{RawPath} not found");
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

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments, char separatorChar)
        {
            //to single string
            var joined = string.Join(separatorChar.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(PathSeparator.Backslash, separatorChar);
            joined = joined.Replace(PathSeparator.Slash, separatorChar);

            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{separatorChar}]{{2,}}", separatorChar.ToString(), RegexOptions.None);

            //to segments
            return joined.Split(new[] { separatorChar }, StringSplitOptions.RemoveEmptyEntries).Where(seg => String.IsNullOrWhiteSpace(seg) == false);
        }

        public static bool DetectIsFolder(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return false;

            return lookingAt.EndsWith(PathSeparator.Backslash) || lookingAt.EndsWith(PathSeparator.Slash);
        }

        private static char DetectPathSeparator(char pathSeparator, IEnumerable<string> segments)
        {
            if (_separatorDetectors.Contains(pathSeparator))
                return pathSeparator;

            if (pathSeparator == PathSeparator.Unknown)
                //auto detect supported separators
                foreach (var segment in segments)
                {
                    if (segment == null)
                        continue;
                    //first separator wins!
                    var separatorIndex = segment.IndexOfAny(_separatorDetectors);
                    if (separatorIndex >= 0)
                        return segment[separatorIndex];
                }

            return PathSeparator.Backslash;//default
        }
    }
}
