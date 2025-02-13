using System;
using System.Collections.Generic;

namespace DotNet.Basics.Win
{
    public class RobocopyStatus
    {
        private static readonly List<string> _statusMessages =
        [
            "No errors occurred, and no copying was done. The source and destination directory trees are completely synchronized.", //0
            "All Okay. One or more files were copied successfully (that is, new files have arrived).", //1
            "Some Extra files or directories were detected. No files were copied. Examine the output log for details.", //2
            "(2 + 1) Some files were copied. Additional files were present. No failure was encountered.", //3
            "Some Mismatched files or directories were detected. Examine the output log. Housekeeping might be required.", //4
            "(4 + 1) Some files were copied. Some files were mismatched. No failure was encountered.", //5
            "(4 + 2) Additional files and mismatched files exist. No files were copied and no failures were encountered. This means that the files already exist in the destination directory", //6
            "(4 + 1 + 2) Files were copied, a file mismatch was present, and additional files were present.", //7
            "ERROR. See log for details. Some files or directories could not be copied.", //8
            "ERROR. See log for details", //9
            "ERROR. See log for details", //10
            "ERROR. See log for details", //11
            "ERROR. See log for details", //12
            "ERROR. See log for details", //13
            "ERROR. See log for details", //14
            "ERROR. See log for details", //15
            "Serious error. Robocopy did not copy any files. Either a usage error or an error due to insufficient access privileges on the source or destination directories."
        ];

        public RobocopyStatus(int exitCode)
        {
            ExitCode = exitCode;
            Failed = ExitCode is >= 8 or < 0;

            try
            {
                StatusMessage = _statusMessages[exitCode];
            }
            catch (ArgumentOutOfRangeException)
            {
                StatusMessage = $"ERROR. Unknown exit code: {exitCode}. See log for details";
            }
        }

        public int ExitCode { get; }
        public bool Failed { get; }
        public string StatusMessage { get; }
    }
}
