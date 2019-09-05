using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public static class LogDispatcherExtensions
    {
        /// <summary>
        /// Will add Azure DevOps console if running on Azure DevOps agent.
        /// IF not, will then try to add ANSI console if supported.
        /// If not, will ultimately fall back to system console (no colors)
        /// </summary>
        /// <param name="log"></param>
        /// <param name="consoleTheme"></param>
        public static void AddFirstSupportedConsole(this ILogDispatcher log, ConsoleTheme consoleTheme = null)
        {
            var SYSTEM_TEAMFOUNDATIONSERVERURI = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONSERVERURI");
            if (SYSTEM_TEAMFOUNDATIONSERVERURI != null && SYSTEM_TEAMFOUNDATIONSERVERURI.Contains("visualstudio.com", StringComparison.InvariantCultureIgnoreCase))
            {
                log.AddDiagnosticsTarget(new AzureDevOpsConsoleWriter());
                log.Verbose($"{typeof(AzureDevOpsConsoleWriter).Name} diagnostics target added");
                return;
            }

            var ansiConsole = new AnsiConsoleWriter(consoleTheme);
            if (ansiConsole.ConsoleModeProperlySet)
            {
                log.AddDiagnosticsTarget(ansiConsole);
                log.Verbose($"{typeof(AnsiConsoleWriter).Name} diagnostics target added");
                return;
            }

            log.AddDiagnosticsTarget(new SystemConsoleWriter());
            log.Verbose($"{typeof(SystemConsoleWriter).Name} diagnostics target added");
        }
    }
}
