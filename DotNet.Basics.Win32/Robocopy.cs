using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Win32
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

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static (int ExitCode, string Output) Run(string source, string target, string filesToCopy = null, string options = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            var command = $"{_fileName} '{source.RemoveSuffix('\\')}' '{target.RemoveSuffix('\\')}'";
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
        public static (int ExitCode, string Output) CopyFile(string sourceFile, string targetDir, string extraOptions = null)
        {
            throw new NotImplementedException();
            /*
            if (targetDir == null) throw new ArgumentNullException(nameof(targetDir));
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentException(nameof(sourceFile));
            var file = sourceFile.ToFile();
            return Run(file.Directory.FullName, targetDir, file.Name, extraOptions ?? "/np");
            */
        }

        /// <summary>
        /// http://ss64.com/nt/robocopy.html
        /// http://ss64.com/nt/robocopy-exit.html
        /// </summary>
        /// <returns>http://ss64.com/nt/robocopy-exit.html</returns>
        public static (int ExitCode, string Output) CopyDir(string sourceDir, string targetDir, bool includeSubFolders = false, string extraOptions = null)
        {
            var options = string.Empty;
            if (includeSubFolders)
                options = _includeSubfoldersOption;
            if (string.IsNullOrWhiteSpace(extraOptions) == false)
                options += $" {extraOptions}";//space in front of options
            return Run(sourceDir, targetDir, null, options);
        }

        public static (int ExitCode, string Output) MoveFolder(string sourceDir, string targetDir, string filter = null, bool recurse = false, string extraOptions = null)
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
        public static (int ExitCode, string Output) MoveContent(string sourceDir, string targetDir, string filter = null, bool recurse = false, string extraOptions = null)
        {
            var result = MoveFolder(sourceDir, targetDir, filter, recurse, extraOptions);
            Directory.CreateDirectory(sourceDir);
            return result;
        }
    }
}
