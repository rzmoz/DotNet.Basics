using Microsoft.Extensions.Logging;
using System.IO;

namespace DotNet.Basics.Diagnostics
{
    public class AdoPipelinesLogFormatter : ILogFormatter
    {
        private static readonly string _successPrefix = "##[section]";

        public void Format(LogLevel level, string? message, TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var adoPrefix = level switch
            {
                LogLevel.Trace => "##[command]",
                LogLevel.Debug => "##[debug]",
                LogLevel.Information => string.Empty,
                LogLevel.Warning => "##vso[task.logissue type=warning;]",
                LogLevel.Error => "##vso[task.logissue type=error;]",
                LogLevel.Critical => "##vso[task.logissue type=error;]",
                _ => string.Empty
            };

            output.Write(adoPrefix);
            output.WriteLine(message.Replace(ConsoleMarkers.SuccessPrefixString, _successPrefix).StripHighlight());
        }
    }
}
