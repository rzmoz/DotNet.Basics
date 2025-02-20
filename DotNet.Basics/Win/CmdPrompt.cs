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
        private StringBuilder _debug = new();
        private StringBuilder _info = new();
        private StringBuilder _error = new();

        public CmdPromptLogger()
        {
            WriteDebug = d => _debug.AppendLine(d);
            WriteInfo = i => _info.AppendLine(i);
            WriteError = e => _error.AppendLine(e);
        }

        public string Debug => _debug.ToString();
        public string Info => _info.ToString();
        public string Error => _error.ToString();

        public Action<string> WriteDebug { get; set; }
        public Action<string> WriteInfo { get; set; }
        public Action<string> WriteError { get; set; }

        public void Reset()
        {
            _debug = new();
            _info = new();
            _error = new();
        }
    }
}
