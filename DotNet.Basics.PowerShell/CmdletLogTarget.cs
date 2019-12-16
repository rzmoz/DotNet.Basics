using System;
using System.Management.Automation;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.PowerShell
{
    public class CmdletLogTarget : ConsoleLogTarget
    {
        private readonly Cmdlet _cmdlet;

        public CmdletLogTarget(Cmdlet cmdlet)
        {
            _cmdlet = cmdlet ?? throw new ArgumentNullException(nameof(cmdlet));
        }

        public override void Write(LogLevel level, string message, Exception e = null)
        {
            var output = base.FormatLogOutput(level, message, e);
            base.WriteFormattedOutput(level, output);
            switch (level)
            {
                case LogLevel.Warning:
                    _cmdlet.WriteWarning(output);
                    break;
                case LogLevel.Error:
                    _cmdlet.WriteError(new ErrorRecord(
                        e ?? new Exception(message),
                        null,
                        ErrorCategory.NotSpecified,
                        message));
                    break;
            }
        }
    }
}
