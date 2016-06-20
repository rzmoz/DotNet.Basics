using System;
using System.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

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
        public static int Run(string source, string target, string filesToCopy = null, string options = null, ILogger logger = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            Init(logger);
            var command = $"{_fullPath} \"{source.RemoveSuffix('\\')}\" \"{target.RemoveSuffix('\\')}\" ";
            if (string.IsNullOrWhiteSpace(filesToCopy) == false)
                command += $" \"{filesToCopy}\" ";
            command += options ?? string.Empty;
            command += " /np /ndl /nfl";//we don't want progress by default
            return CommandPrompt.Run(command, logger);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyFile(string sourceFile, string targetDir, string extraOptions = null, ILogger logger = null)
        {
            if (targetDir == null) throw new ArgumentNullException(nameof(targetDir));
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentException(nameof(sourceFile));
            var file = sourceFile.ToFile();
            return Run(file.Directory.FullName, targetDir, file.Name, extraOptions ?? "/np", logger);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int CopyDir(string sourceDir, string targetDir, bool includeSubFolders = false, string extraOptions = null, ILogger logger = null)
        {
            var options = string.Empty;
            if (includeSubFolders)
                options = _includeSubfoldersOption;
            if (string.IsNullOrWhiteSpace(extraOptions) == false)
                options += $" {extraOptions}";//space in front of options
            return Run(sourceDir, targetDir, null, options, logger);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static int MoveFiles(string sourceDir, string targetDir, string file = null, string extraOptions = null, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(file))
                return Run(sourceDir, targetDir, file, $"{_includeSubfoldersOption} {_moveOption} {extraOptions}");//move dir
            return Run(sourceDir, targetDir, file, _moveOption, logger);
        }

        private static void Init(ILogger logger = null)
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
                logger?.LogDebug("Robocopy found at: " + _fullPath);
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
                    Debug.WriteLine("Looking for robocopy in {0}", lookFor);
                    if (lookFor.ToFile().Exists())
                        return lookFor;
                }
            }

            throw new System.IO.IOException("Robocopy not found");
        }
    }
}
