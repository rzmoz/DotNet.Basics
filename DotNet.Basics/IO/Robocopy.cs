using System;
using System.Diagnostics;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    /// <summary>
    /// http://ss64.com/nt/robocopy.html
    /// http://ss64.com/nt/robocopy-exit.html
    /// </summary>
    public static class Robocopy
    {
        private const string _fileName = @"Robocopy";
        private const string _includeSubfoldersOption = " /e ";
        private const string _moveOption = " /move ";
        private static string _fullPath = null;
        private static readonly object _syncRoot = new Object();

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int Run(string source, string target, string filesToCopy = null, string options = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            Init();
            var command = $"{_fullPath} \"{source}\" \"{target}\"";
            if (string.IsNullOrWhiteSpace(filesToCopy) == false)
                command += $" \"{filesToCopy}\"";
            command += options ?? string.Empty;
            return CommandPrompt.Run(command);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyFile(string sourceDir, string targetDir, string file, string extraOptions = null)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException(nameof(file));
            return Run(sourceDir, targetDir, file, extraOptions);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyDir(string sourceDir, string targetDir, DirCopyOptions dirCopyOptions = DirCopyOptions.IncludeSubDirectories, string extraOptions = null)
        {
            var options = string.Empty;
            if (dirCopyOptions == DirCopyOptions.IncludeSubDirectories)
                options = _includeSubfoldersOption;
            if (string.IsNullOrWhiteSpace(extraOptions) == false)
                options += " " + extraOptions;
            return Run(sourceDir, targetDir, null, options);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int Move(string sourceDir, string targetDir, string file = null, string extraOptions = null)
        {
            if (string.IsNullOrEmpty(file))
                return Run(sourceDir, targetDir, file, $"{_includeSubfoldersOption} {_moveOption} {extraOptions}");//move dir
            return Run(sourceDir, targetDir, file, _moveOption);
        }

        private static void Init()
        {
            if (_fullPath != null)
                return;

            lock (_syncRoot)
            {
                var lookforPaths = new[]
                {
                    $@"\Windows\SysWOW64\{_fileName}.exe",
                    $@"\Windows\System32\{_fileName}.exe"
                };

                _fullPath = LookupRobocopy(lookforPaths);
                Debug.WriteLine("Robocopy found at: " + _fullPath);
            }
        }

        private static string LookupRobocopy(params string[] searchPaths)
        {
            //see if robocopy is registered as an internal command
            var exitCode = CommandPrompt.Run(_fileName);

            //exit code means that robocopy actually ran which means we got it
            if (exitCode == 16)
                return _fileName;

            var drives = DriveInfo.GetDrives();
            foreach (var driveInfo in drives)
            {
                foreach (var searchPath in searchPaths)
                {
                    var lookFor = driveInfo.Name + searchPath.TrimStart('\\');
                    Debug.WriteLine("Looking for robocopy in {0}", lookFor);
                    if (File.Exists(lookFor))
                        return lookFor;
                }
            }

            throw new IOException("Robocopy not found");
        }
    }
}
