using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DotNet.Basics.Cli.Logging
{
    public class ConsoleStyleTheme(ConsoleStyleSet primary, ConsoleStyleSet? highlights = null)
    {
        public Style GetStyle(LogLevel level, bool isSuccess = false, bool isHighlight = false)
        {
            if (isSuccess)
                return isHighlight ? highlights?.Success ?? primary.Success: primary.Success;

            return level switch
            {
                LogLevel.Trace => isHighlight ? highlights?.Trace ?? primary.Trace : primary.Trace,
                LogLevel.Debug => isHighlight ? highlights?.Debug ?? primary.Debug : primary.Debug,
                LogLevel.Information => isHighlight ? highlights?.Info ?? primary.Info : primary.Info,
                LogLevel.Warning => isHighlight ? highlights?.Warning ?? primary.Warning : primary.Warning,
                LogLevel.Error => isHighlight ? highlights?.Error ?? primary.Error : primary.Error,
                LogLevel.Critical => isHighlight ? highlights?.Critical ?? primary.Critical : primary.Critical,
                _ => isHighlight ? highlights?.Default ?? primary.Default : primary.Default
            };
        }
    }
}
