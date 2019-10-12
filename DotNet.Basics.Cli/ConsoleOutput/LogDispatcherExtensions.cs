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
        public static ILogDispatcher AddFirstSupportedConsole(this ILogDispatcher log, ConsoleTheme consoleTheme = null)
        {
            if (AzureDevOpsConsoleWriter.EnvironmentIsAzureDevOpsHostedAgent())
                return log.AddAzureDevOpsConsole();
            if (AnsiConsoleWriter.IsSupported)
                return log.AddAnsiConsole(consoleTheme, false);
            return log.AddSystemConsole();
        }

        public static ILogDispatcher AddAzureDevOpsConsole(this ILogDispatcher log)
        {
            return AddDiagnosticsTarget(log, new AzureDevOpsConsoleWriter());
        }
        public static ILogDispatcher AddAnsiConsole(this ILogDispatcher log, ConsoleTheme consoleTheme = null, bool logIfNotSupported = true)
        {
            var ansiConsole = new AnsiConsoleWriter(consoleTheme);
            if (AnsiConsoleWriter.IsSupported)
                return AddDiagnosticsTarget(log, ansiConsole);
            if (logIfNotSupported)
                log.Error($"{nameof(AnsiConsoleWriter)} is not supported. This means that this environment does not support this ANSI console so it was NOT added as a diagnostics target!!");
            return log;
        }
        public static ILogDispatcher AddSystemConsole(this ILogDispatcher log)
        {
            return AddDiagnosticsTarget(log, new SystemConsoleWriter());
        }

        public static ILogDispatcher AddDiagnosticsTarget(ILogDispatcher log, IDiagnosticsTarget target)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (target == null) throw new ArgumentNullException(nameof(target));
            log.AddDiagnosticsTarget(target);
            log.Verbose($"{target.GetType().Name} diagnostics target added");
            return log;
        }
    }
}
