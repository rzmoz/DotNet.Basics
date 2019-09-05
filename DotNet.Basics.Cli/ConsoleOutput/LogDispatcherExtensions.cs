using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public static class LogDispatcherExtensions
    {
        /// <summary>
        /// Will try to add ANSI console if supported.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="consoleTheme"></param>
        public static void AddConsole(this ILogDispatcher log, ConsoleTheme consoleTheme = null)
        {
            IDiagnosticsTarget console = new AnsiConsoleWriter(consoleTheme);
            if (((AnsiConsoleWriter)console).ConsoleModeProperlySet == false)
                console = new SystemConsoleWriter();
            log.AddDiagnosticsTarget(console);
        }
    }
}
