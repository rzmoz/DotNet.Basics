using System.Collections.Generic;

namespace DotNet.Basics.IO
{
    /// <summary>
    /// http://ss64.com/nt/robocopy.html
    /// http://ss64.com/nt/robocopy-exit.html
    /// </summary>
    public class RobocopyStatus
    {
        private static readonly List<string> _statusMessages = new List<string>
        {
            "No errors occurred, and no copying was done. The source and destination directory trees are completely synchronized.",//0
            "All Okay. One or more files were copied successfully (that is, new files have arrived).",//1
            "Some Extra files or directories were detected. No files were copied. Examine the output log for details.",//2
            "(2 + 1) Some files were copied. Additional files were present. No failure was encountered.",//3
            "Some Mismatched files or directories were detected. Examine the output log. Housekeeping might be required.",//4
            "(4 + 1) Some files were copied. Some files were mismatched. No failure was encountered.",//5
            "(4 + 2) Additional files and mismatched files exist. No files were copied and no failures were encountered. This means that the files already exist in the destination directory",//6
            "(4 + 1 + 2) Files were copied, a file mismatch was present, and additional files were present.",//7
            "ERROR. See log for details. Some files or directories could not be copied.",//8
            "ERROR. See log for details",//9
            "ERROR. See log for details",//10
            "ERROR. See log for details",//11
            "ERROR. See log for details",//12
            "ERROR. See log for details",//13
            "ERROR. See log for details",//14
            "ERROR. See log for details",//15
            "Serious error. Robocopy did not copy any files. Either a usage error or an error due to insufficient access privileges on the source or destination directories."//16
        };

        public RobocopyStatus(int exitCode)
        {
            if (exitCode < 0 || exitCode > 16)
                exitCode = 16;//set to serious error if exit code is not understood
            ExitCode = exitCode;
            Failed = ExitCode >= 8;
            StatusMessage = _statusMessages[exitCode];

        }

        public int ExitCode { get; }
        public bool Failed { get; }
        public string StatusMessage { get; }
    }
}
