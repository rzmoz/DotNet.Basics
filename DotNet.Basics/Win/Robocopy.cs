using DotNet.Basics.Sys;
using System;
using System.IO;

namespace DotNet.Basics.Win
{
    public static class Robocopy
    {
        private const string _fileName = @"Robocopy";
        private const string _includeSubfoldersOption = " /e ";
        private const string _moveOption = " /move ";

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static RobocopyStatus Run(string source, string target, string? filesToCopy = null, string? options = " /NS /NC /NFL /NDL /NP", Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            var command = $@"{_fileName} ""{source.RemoveSuffix('\\')}"" ""{target.RemoveSuffix('\\')}""";
            if (string.IsNullOrWhiteSpace(filesToCopy) == false)
                command += $" \"{filesToCopy}\" ";
            command += options ?? string.Empty;
            var exitCode = CmdPrompt.Run(command, writeOutput, writeError, writeDebug);
            return new RobocopyStatus(exitCode);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static RobocopyStatus CopyFile(string sourceDir, string targetDir, string sourceFileName, string? extraOptions = null, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            if (sourceFileName == null) throw new ArgumentNullException(nameof(sourceFileName));
            if (targetDir == null) throw new ArgumentNullException(nameof(targetDir));
            if (string.IsNullOrEmpty(sourceDir)) throw new ArgumentException(nameof(sourceDir));
            return Run(sourceDir, targetDir, sourceFileName, extraOptions ?? "/np", writeOutput, writeError, writeDebug);
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static RobocopyStatus CopyDir(string sourceDir, string targetDir, bool includeSubFolders = false, string? extraOptions = null, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            var options = string.Empty;
            if (includeSubFolders)
                options = _includeSubfoldersOption;
            if (string.IsNullOrWhiteSpace(extraOptions) == false)
                options += $" {extraOptions}";//space in front of options
            return Run(sourceDir, targetDir, null, options, writeOutput, writeError, writeDebug);
        }

        public static RobocopyStatus MoveFolder(string sourceDir, string targetDir, string? filter = null, bool includeSubFolders = false, string? extraOptions = null, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            string options = $"{_moveOption} {extraOptions}";
            if (includeSubFolders)
                options = $"{_includeSubfoldersOption} {options}";
            var moveResult = Run(sourceDir, targetDir, filter, options, writeOutput, writeError, writeDebug);
            return moveResult;
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static RobocopyStatus MoveContent(string sourceDir, string targetDir, string? filter = null, bool includeSubFolders = false, string? extraOptions = null, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            var result = MoveFolder(sourceDir, targetDir, filter, includeSubFolders, extraOptions, writeOutput, writeError, writeDebug);
            Directory.CreateDirectory(sourceDir);
            return result;
        }
    }
}
