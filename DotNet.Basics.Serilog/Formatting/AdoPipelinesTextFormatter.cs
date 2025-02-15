using System.IO;
using DotNet.Basics.Serilog.Looging;
using Serilog.Events;
using Serilog.Formatting;

namespace DotNet.Basics.Serilog.Formatting
{
    public class AdoPipelinesTextFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            var adoPrefix = logEvent.Level switch
            {
                LogEventLevel.Verbose => "##[command]",
                LogEventLevel.Debug => "##[debug]",
                LogEventLevel.Information => string.Empty,
                LogEventLevel.Warning => "##vso[task.logissue type=warning;]",
                LogEventLevel.Error => "##vso[task.logissue type=error;]",
                LogEventLevel.Fatal => "##vso[task.logissue type=error;]",
                _ => string.Empty
            };

            output.Write(adoPrefix);
            output.WriteLine(logEvent.MessageTemplate.Text.StripHighlight());
        }
    }
}
