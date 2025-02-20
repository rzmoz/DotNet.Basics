using DotNet.Basics.Sys;
using System;

namespace DotNet.Basics.Win
{
    public static class CmdPrompt
    {
        public static int Run(string commandString, CmdPromptLogger? logger = null)
        {
            logger ??= new CmdPromptLogger();
            return Run(commandString, logger.WriteInfo, logger.WriteError, logger.WriteDebug);
        }

        public static int Run(string commandString, Action<string> writeOutput, Action<string> writeError, Action<string> writeDebug)
        {
            return ExternalProcess.Run("cmd.exe", $"/c {commandString}", writeOutput, writeError, writeDebug);
        }
    }
}
