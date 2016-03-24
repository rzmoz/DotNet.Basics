﻿using System;
using System.Diagnostics;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    /// <summary>
    /// http://ss64.com/nt/robocopy-exit.html
    /// </summary>
    public static class Robocopy
    {
        private const string _fileName = @"Robocopy";
        private const string _sourceDestinationFormat = @" ""{0}"" ""{1}"" /NP ";
        private const string _includeSubfoldersSwitch = " /e ";
        private const string _moveSwitch = " /move ";
        private static string _fullPath = null;
        private static readonly object _syncRoot = new Object();

        /// <summary>
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyFile(string sourceDir, string targetDir, string file = null)
        {
            return Run(sourceDir, targetDir, file ?? string.Empty);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyDir(string sourceDir, string targetDir, DirCopyOptions dirCopyOptions = DirCopyOptions.IncludeSubDirectories)
        {
            var switches = string.Empty;
            if (dirCopyOptions == DirCopyOptions.IncludeSubDirectories)
                switches = _includeSubfoldersSwitch;
            return Run(sourceDir, targetDir, switches);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int Move(string sourceDir, string targetDir, string file = null)
        {
            if (string.IsNullOrEmpty(file))
                return Run(sourceDir, targetDir, _includeSubfoldersSwitch + _moveSwitch);//move dir
            return Run(sourceDir, targetDir, file + _moveSwitch);
        }

        private static void Init()
        {
            if (_fullPath != null)
                return;

            lock (_syncRoot)
            {
                var lookforPaths = new string[]
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

        private static int Run(string source, string target, string switches)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            Init();
            var command = string.Format(_fullPath + _sourceDestinationFormat, source, target);
            command += switches;
            return CommandPrompt.Run(command);
        }
    }
}
