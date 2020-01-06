using System.Diagnostics;
using System.Management.Automation;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Diagnostics.Console;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

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
        protected DirPath Location => SessionState.Path.CurrentFileSystemLocation.Path.ToDir();
        protected ILogger Log { get; }

        [Parameter]
        [Alias("d")]
        public SwitchParameter PauseForDebugger { get; set; }

        protected void PauseForDebug()
        {
            if (MyInvocation.BoundParameters.ContainsKey("Debug") || PauseForDebugger.IsPresent)
            {
                Log.Raw($"Paused for attaching debugger. Process name: {Process.GetCurrentProcess().ProcessName} | PID: {Process.GetCurrentProcess().Id}. Press [ENTER] to continue..");
                Host.UI.ReadLine();
            }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            PauseForDebug();
        }
    }
}
