﻿using System;
using System.Diagnostics;
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
        private static readonly object _syncRoot = new object();

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
            var command = $"{_fullPath} \"{source.RemoveSuffix('\\')}\" \"{target.RemoveSuffix('\\')}\" ";
            if (string.IsNullOrWhiteSpace(filesToCopy) == false)
                command += $" \"{filesToCopy}\" ";
            command += options ?? string.Empty;
            command += " /np /ndl /nfl";//we don't want progress by default
            return CommandPrompt.Run(command);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyFile(string sourceFile, string targetDir, string extraOptions = null)
        {
            if (targetDir == null) throw new ArgumentNullException(nameof(targetDir));
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentException(nameof(sourceFile));
            var file = sourceFile.ToFile();
            return Run(file.Directory.FullName, targetDir, file.Name, extraOptions ?? "/np");
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyDir(string sourceDir, string targetDir, bool includeSubFolders = false, string extraOptions = null)
        {
            var options = string.Empty;
            if (includeSubFolders)
                options = _includeSubfoldersOption;
            if (string.IsNullOrWhiteSpace(extraOptions) == false)
                options += $" {extraOptions}";//space in front of options
            return Run(sourceDir, targetDir, null, options);
        }


        public static int MoveFolder(string sourceDir, string targetDir, string filter = null, bool recurse = false, string extraOptions = null)
        {
            string options = $"{_moveOption} {extraOptions}";
            if (recurse)
                options = $"{_includeSubfoldersOption} {options}";
            var moveResult = Run(sourceDir, targetDir, filter, options);
            return moveResult;
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int MoveContent(string sourceDir, string targetDir, string filter = null, bool recurse = false, string extraOptions = null)
        {
            var result = MoveFolder(sourceDir, targetDir, filter, recurse, extraOptions);
            sourceDir.ToDir().CreateIfNotExists();
            return result;
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
                DebugOut.WriteLine($"Robocopy found at: {_fullPath}");
            }
        }

        private static string LookupRobocopy(params string[] searchPaths)
        {
            //see if robocopy is registered as an internal command
            var exitCode = CommandPrompt.Run(_fileName + " /njh /njs");

            //exit code means that robocopy actually ran which means we got it
            if (exitCode == 16)
                return _fileName;

            var drives = System.IO.DriveInfo.GetDrives();
            foreach (var driveInfo in drives)
            {
                foreach (var searchPath in searchPaths)
                {
                    var lookFor = driveInfo.Name + searchPath.TrimStart('\\');
                    DebugOut.WriteLine($"Looking for robocopy in {lookFor}");
                    if (lookFor.ToFile().Exists())
                        return lookFor;
                }
            }

            throw new System.IO.IOException("Robocopy not found");
        }
    }
}
