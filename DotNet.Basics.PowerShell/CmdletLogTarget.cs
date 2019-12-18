using System;
using System.Management.Automation;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.PowerShell
{
    public class CmdletLogTarget : AnsiConsoleLogTarget
    {
        private readonly Cmdlet _cmdlet;

        public CmdletLogTarget(Cmdlet cmdlet)
        {
            _cmdlet = cmdlet ?? throw new ArgumentNullException(nameof(cmdlet));
        }

        public override void Write(LogLevel level, string message, Exception e = null)
        {
            var output = base.FormatLogOutput(level, message, e);

            switch (level)
            {
                case LogLevel.Warning:
                    _cmdlet.WriteWarning(output.StripHighlight());
                    break;
                case LogLevel.Error:
                    _cmdlet.WriteError(new ErrorRecord(
                        e ?? new Exception(message.StripHighlight()),
                        null,
                        ErrorCategory.NotSpecified,
                        message.StripHighlight()));
                    break;
                default:
                    base.WriteFormattedOutput(level, output);
                    break;
            }
        }
    }
}
