using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class AzureDevOpsConsoleWriter : ConsoleWriter
    {
        protected override string FormatLogLevel(LogLevel level)
        {
            return OutputColorPrefix(level);
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
                case LogLevel.Vrb:
                    return "##[command]";
                case LogLevel.Dbg:
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
