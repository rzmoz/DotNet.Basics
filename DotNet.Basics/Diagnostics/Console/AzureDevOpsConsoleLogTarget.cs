using System;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics.Console
{
    public class AzureDevOpsConsoleLogTarget : ConsoleLogTarget
    {
        protected override string FormatLogLevel(LogLevel level)
        {
            return OutputColorPrefix(level);
        }

        public override void Write(LogLevel level, string message, Exception e = null)
        {
            lock (SyncRoot)
            {
                var console = level < LogLevel.Error ? System.Console.Out : System.Console.Error;

                message.ToMultiLine()
                    .ForEach(line =>
                    {
                        var output = FormatLogOutput(level, line).StripHighlight();
                        console.Write(output);
                        console.Flush();
                    });

                if (e != null)
                {
                    var output = FormatLogOutput(level, null, e).StripHighlight();
                    console.Write(output);
                    console.Flush();
                }
            }
        }

        public static bool EnvironmentIsAzureDevOpsHostedAgent()
        {
            var SYSTEM_TEAMFOUNDATIONSERVERURI = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONSERVERURI");
            return SYSTEM_TEAMFOUNDATIONSERVERURI != null &&
                   SYSTEM_TEAMFOUNDATIONSERVERURI.ToLowerInvariant().Contains("visualstudio.com");
        }

        private static string OutputColorPrefix(LogLevel level) =>
            level switch
            {
                LogLevel.Verbose => "##[command]",
                LogLevel.Debug => "##[debug]",
                LogLevel.Info => string.Empty,
                LogLevel.Success => "##[section]",
                LogLevel.Warning => "##vso[task.logissue type=warning;]",
                LogLevel.Error => "##vso[task.logissue type=error;]",
                _ => string.Empty
            };
    }
}
