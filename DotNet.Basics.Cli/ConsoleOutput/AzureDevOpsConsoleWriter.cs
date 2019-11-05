using System;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class AzureDevOpsConsoleWriter : ConsoleWriter
    {
        protected override string FormatLogLevel(LogLevel level)
        {
            return OutputColorPrefix(level);
        }

        public override void Write(LogLevel level, string message, Exception e = null)
        {
            lock (SyncRoot)
            {
                var console = level < LogLevel.Error ? Console.Out : Console.Error;

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
                    return "##vso[task.logissue type=error;]";
                default:
                    return string.Empty;
            }
        }
    }
}
