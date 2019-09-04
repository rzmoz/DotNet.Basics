using System;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

        public static object[] Run(string name, Action<PowerShellCmdlet> addParams = null)
        {
            return Run(new VoidLogger(), name, addParams);
        }
        public static object[] Run(ILogDispatcher log, string name, Action<PowerShellCmdlet> addParams = null)
        {
            var cmdLet = new PowerShellCmdlet(name);
            addParams?.Invoke(cmdLet);
            return Run(log, cmdLet);
        }

        public static object[] Run(PowerShellCmdlet cmdLet)
        {
            return Run(new VoidLogger(), cmdLet);
        }
        public static object[] Run(ILogDispatcher log, PowerShellCmdlet cmdLet)
        {
            return Run(log, cmdLet.ToString());
        }

        public static object[] Run(params string[] scripts)
        {
            return Run(new VoidLogger(), scripts);
        }

        public static object[] Run(ILogDispatcher log, params string[] scripts)
        {
            if (log == null)
                log = new VoidLogger();

            using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create())
            {
                ps.AddScript(_bypassExecutionPolicy);
                foreach (var script in scripts)
                    ps.AddScript(script);

                ps.Streams.Progress.DataAdded += (col, e) => log.Verbose($"{((PSDataCollection<ProgressRecord>)col).Last().Activity} : {((PSDataCollection<ProgressRecord>)col).Last().PercentComplete}/100");
                ps.Streams.Verbose.DataAdded += (col, e) => log.Verbose(((PSDataCollection<VerboseRecord>)col).Last().Message);
                ps.Streams.Debug.DataAdded += (col, e) => log.Information(((PSDataCollection<DebugRecord>)col).Last().Message);
                ps.Streams.Information.DataAdded += (col, e) => log.Information(((PSDataCollection<InformationRecord>)col).Last().ToString());
                ps.Streams.Warning.DataAdded += (col, e) => log.Warning(((PSDataCollection<WarningRecord>)col).Last().Message);
                ps.Streams.Error.DataAdded += (col, e) =>
                {
                    var record = ((PSDataCollection<ErrorRecord>)col).Last();
                    if (record.Exception is RemoteException)
                        log.Warning(record.Exception.Message);
                    else
                        log.Error(record.Exception.Message, record.Exception);
                };

                var passThru = ps.Invoke();

                var realErrors = ps.Streams.Error.Select(rec => rec.Exception).Where(e => e != null && e is RemoteException == false).ToList();

                if (realErrors.Any())
                {
                    if (realErrors.Count == 1)
                        throw realErrors.First();
                    throw new AggregateException(realErrors);
                }
                return passThru?.Select(o => o.BaseObject).ToArray();
            }
        }
    }
}