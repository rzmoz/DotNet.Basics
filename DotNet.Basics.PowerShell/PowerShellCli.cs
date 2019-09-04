using System;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

        public static object[] RunCmdlet(PowerShellCmdlet cmdLet, ILogDispatcher log = null)
        {
            return RunScript(log, cmdLet.ToString());
        }

        public static object[] RunScript(params string[] scripts)
        {
            return RunScript(new VoidLogger(), scripts);
        }

        public static object[] RunScript(ILogDispatcher log, params string[] scripts)
        {
            if (log == null)
                log = new VoidLogger();

            using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create())
            {
                ps.AddScript(_bypassExecutionPolicy);
                foreach (var script in scripts)
                    ps.AddScript(script);

                ps.Streams.Progress.DataAdded += (col, e) => ((PSDataCollection<ProgressRecord>)col).ForEach(rec => log.Verbose($"{rec.Activity} : {rec.PercentComplete}/100"));
                ps.Streams.Verbose.DataAdded += (col, e) => ((PSDataCollection<VerboseRecord>)col).ForEach(rec => log.Verbose(rec.Message));
                ps.Streams.Debug.DataAdded += (col, e) => ((PSDataCollection<DebugRecord>)col).ForEach(rec => log.Debug(rec.Message)); ;
                ps.Streams.Information.DataAdded += (col, e) => ((PSDataCollection<InformationRecord>)col).ForEach(rec => log.Information(rec.ToString())); ;
                ps.Streams.Warning.DataAdded += (col, e) => ((PSDataCollection<WarningRecord>)col).ForEach(rec => log.Warning(rec.Message)); ;
                ps.Streams.Error.DataAdded += (col, e) => ((PSDataCollection<ErrorRecord>)col).ForEach(rec => log.Error($"{rec.Exception?.Message}", rec.Exception));

                var passThru = ps.Invoke();

                if (ps.Streams.Error.Any())
                {
                    if (ps.Streams.Error.Count == 1)
                        throw ps.Streams.Error.Single().Exception;
                    throw new AggregateException(ps.Streams.Error.Select(e => e.Exception ?? new CmdletInvocationException(e.ErrorDetails?.ToString())));
                }

                return passThru?.Select(o => o.BaseObject).ToArray();
            }
        }
    }
}