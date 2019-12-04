using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Will add Azure DevOps console if running on Azure DevOps agent.
        /// IF not, will then try to add ANSI console if supported.
        /// If not, will ultimately fall back to system console (no colors)
        /// </summary>
        /// <param name="log"></param>
        /// <param name="consoleTheme"></param>
        public static ILogger AddFirstSupportedConsole(this ILogger log, ConsoleTheme consoleTheme = null)
        {
            if (AzureDevOpsConsoleLogTarget.EnvironmentIsAzureDevOpsHostedAgent())
                return log.AddAzureDevOpsConsole();
            if (AnsiConsoleLogTarget.IsSupported)
                return log.AddAnsiConsole(consoleTheme, false);
            return log.AddSystemConsole();
        }

        public static ILogger AddAzureDevOpsConsole(this ILogger log)
        {
            return AddDiagnosticsTarget(log, new AzureDevOpsConsoleLogTarget());
        }
        public static ILogger AddAnsiConsole(this ILogger log, ConsoleTheme consoleTheme = null, bool logIfNotSupported = true)
        {
            var ansiConsole = new AnsiConsoleLogTarget(consoleTheme);
            if (AnsiConsoleLogTarget.IsSupported)
                return AddDiagnosticsTarget(log, ansiConsole);
            if (logIfNotSupported)
                log.Error($"{nameof(AnsiConsoleLogTarget)} is not supported. This means that this environment does not support this ANSI console so it was NOT added as a diagnostics target!!");
            return log;
        }
        public static ILogger AddSystemConsole(this ILogger log)
        {
            return AddDiagnosticsTarget(log, new SystemConsoleLogTarget());
        }

        public static ILogger AddDiagnosticsTarget(ILogger log, ILogTarget target)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (target == null) throw new ArgumentNullException(nameof(target));
            log.AddLogTarget(target);
            log.Verbose($"{target.GetType().Name} diagnostics target added");
            return log;
        }
    }
}
