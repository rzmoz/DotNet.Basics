using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

        public static int RunFileInConsole(string fileArgs, ILogDispatcher log = null)
        {
            if (log == null)
                log = new VoidLogger();

            var errors = new StringBuilder();

            var exitCode = CmdPrompt.Run($"PowerShell -ExecutionPolicy bypass -NonInteractive -NoProfile -file {fileArgs}", log.Information, error => { errors.AppendLine(error); log.Error(error); });
            if (exitCode == -196608)
                throw new FileNotFoundException(fileArgs);

            if (errors.Length > 0)
                throw new PowerShellException(errors.ToString(), exitCode);

            return exitCode;
        }

        public static object[] RunCmdlet(PowerShellCmdlet cmdLet, ILogDispatcher log = null)
        {
            return RunScript(cmdLet.ToString(), log);
        }

        public static object[] RunScript(string script, ILogDispatcher log = null)
        {
            if (log == null)
                log = new VoidLogger();
            var ps = System.Management.Automation.PowerShell.Create();
            ps.AddScript(_bypassExecutionPolicy);
            ps.AddScript(script);

            ps.Streams.Progress.DataAdded += (col, e) => log.Verbose($"{((PSDataCollection<ProgressRecord>)col).Last().Activity} : {((PSDataCollection<ProgressRecord>)col).Last().PercentComplete}/100");
            ps.Streams.Verbose.DataAdded += (col, e) => log.Verbose(((PSDataCollection<VerboseRecord>)col).Last().Message);
            ps.Streams.Debug.DataAdded += (col, e) => log.Information(((PSDataCollection<DebugRecord>)col).Last().Message);
            ps.Streams.Information.DataAdded += (col, e) => log.Information(((PSDataCollection<InformationRecord>)col).Last().ToString());
            ps.Streams.Warning.DataAdded += (col, e) => log.Warning(((PSDataCollection<WarningRecord>)col).Last().Message);
            ps.Streams.Error.DataAdded += (col, e) => log.Error(((PSDataCollection<ErrorRecord>)col).Last().Exception.Message);

            var passThru = ps.Invoke();

            if (ps.HadErrors)
            {
                throw new RemoteException($"{"PowerShell scripts failed:".Highlight()}\r\n{ps.Streams.Error.Select(e => e.Exception.Message).JoinString("\r\n")}");
            }

            return passThru?.Select(o => o.BaseObject).ToArray();
        }
    }
}