using DotNet.Basics.Sys;
using System;

namespace DotNet.Basics.Win
{
    public static class CmdPrompt
    {
        public static int Run(string commandString, CmdPromptLogger logger)
        {
            return Run(commandString, logger?.WriteOutput, logger?.WriteError, logger?.WriteDebug);
        }

        public static int Run(string commandString, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            return ExternalProcess.Run("cmd.exe", $"/c {commandString}", writeOutput, writeError, writeDebug);
        }
    }
}
