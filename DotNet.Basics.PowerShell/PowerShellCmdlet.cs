using System.Management.Automation;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.PowerShell
{
    public class PowerShellCmdlet : PSCmdlet
    {
        protected PowerShellCmdlet(string cmdletName = null)
        {
            CmdletName = cmdletName ?? GetType().Name;
            Log = new Logger(CmdletName);
            Log.AddLogTarget(new CmdletLogTarget(this));
        }

        public string CmdletName { get; }

        protected ILogger Log { get; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            Log.Verbose($"Processing Cmdlet {CmdletName} started");
        }

        /// <summary>
        /// Add to the end of your method if you override since it contains profiling code that should embrace all execution paths in derived cmdlet
        /// </summary>
        protected override void EndProcessing()
        {
            base.EndProcessing();
            Log.Verbose($"Processing Cmdlet {CmdletName} ended");
        }
    }
}
