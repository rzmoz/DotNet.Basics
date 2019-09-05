using System;
using System.Text;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class AzureDevOpsConsoleWriter : ConsoleWriter
    {
        public override void Write(LogLevel level, string message, Exception e = null)
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append($"{OutputColorPrefix(level)} ");
            outputBuilder.Append($"{message}");
            if (!(e is CliException exception && exception.LogOptions == LogOptions.ExcludeStackTrace))
                outputBuilder.Append($"\r\n{e}");
            var output = outputBuilder.ToString().StripHighlight();

            Console.Write(output);
        }

        private static string OutputColorPrefix(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return "##[command]";
                case LogLevel.Debug:
                    return "##[debug]";
                case LogLevel.Info:
                    return string.Empty;
                case LogLevel.Success:
                    return "##[section]";
                case LogLevel.Warning:
                    return "##vso[task.logissue type=warning;]";
                case LogLevel.Error:
                case LogLevel.Critical:
                    return "##vso[task.logissue type=error;]";
                case LogLevel.None:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
