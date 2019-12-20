using System.Management.Automation;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.PowerShell
{
    public abstract class PowerShellCmdlet : PSCmdlet
    {
        protected PowerShellCmdlet(params ILogTarget[] logTargets)
        : this(true, logTargets)
        { }
        protected PowerShellCmdlet(bool addFirstSupportedConsole, params ILogTarget[] logTargets)
        {
            CmdletName = GetType().Name;
            Log = new Logger(CmdletName);

            if (addFirstSupportedConsole)
                Log.AddFirstSupportedConsole();
            foreach (var logTarget in logTargets)
                Log.AddLogTarget(logTarget);
        }

        public string CmdletName { get; }

        protected ILogger Log { get; }
    }
}
