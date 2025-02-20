using DotNet.Basics.Sys;
using System;
using System.Text;

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

    public class CmdPromptLogger
    {
        public StringBuilder Debug { get; set; } = new();
        public StringBuilder Info { get; set; } = new();
        public StringBuilder Error { get; set; } = new();

        public CmdPromptLogger()
        {
            WriteDebug = d => Debug.AppendLine(d);
            WriteInfo = i => Info.AppendLine(i);
            WriteError = e => Error.AppendLine(e);
        }

        public Action<string> WriteDebug { get; set; }
        public Action<string> WriteInfo { get; set; }
        public Action<string> WriteError { get; set; }

        public void Reset()
        {
            Debug = new();
            Info = new();
            Error = new();
        }
    }
}
