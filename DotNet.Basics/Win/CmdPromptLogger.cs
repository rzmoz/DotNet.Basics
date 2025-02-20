using System;
using System.Text;

namespace DotNet.Basics.Win
{
    public class CmdPromptLogger
    {
        public delegate void DebugLoggedEventHandler(string line);
        public delegate void InfoLoggedEventHandler(string line);
        public delegate void ErrorLoggedEventHandler(string line);

        public event DebugLoggedEventHandler? DebugLogged;
        public event InfoLoggedEventHandler? InfoLogged;
        public event ErrorLoggedEventHandler? ErrorLogged;

        public StringBuilder Debug { get; set; } = new();
        public StringBuilder Info { get; set; } = new();
        public StringBuilder Error { get; set; } = new();

        public CmdPromptLogger()
        {
            WriteDebug = d =>
            {
                Debug.AppendLine(d);
                DebugLogged?.Invoke(d);
            };
            WriteInfo = i =>
            {
                Info.AppendLine(i);
                InfoLogged?.Invoke(i);
            };
            WriteError = e =>
            {
                Error.AppendLine(e);
                ErrorLogged?.Invoke(e);
            };
        }

        public Action<string> WriteDebug { get; }
        public Action<string> WriteInfo { get; }
        public Action<string> WriteError { get; }

        public void Reset()
        {
            Debug = new();
            Info = new();
            Error = new();
        }
    }
}
