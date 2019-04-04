using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class ColoredConsoleWriter
    {
        private readonly object _syncRoot = new object();
        private readonly ColoredConsoleTheme _consoleTheme;
        private readonly Func<LogLevel, string, Exception, string> _format;
        public ColoredConsoleWriter(bool includeTimestamp = false, ColoredConsoleTheme consoleTheme = null)
        {
            _consoleTheme = consoleTheme ?? new ColoredConsoleTheme();

            if (includeTimestamp)
                _format = WithTimestamp;
            else
                _format = WithoutTimestamp;
        }

        private string WithTimestamp(LogLevel level, string message, Exception e = null)
        {
            return $"{DateTime.Now:s} [{WriteLogLevel(level)}] {message}\r\n{e}";
        }
        private string WithoutTimestamp(LogLevel level, string message, Exception e = null)
        {
            return $"[{WriteLogLevel(level)}] {message}\r\n{e}";
        }

        public void Write(LogLevel level, string message, Exception e = null)
        {
            var colors = GetColors(level);

            lock (_syncRoot)
            {
                Console.ForegroundColor = colors.Foreground;
                if (colors.Background != ConsoleColor.Black)
                    Console.BackgroundColor = colors.Background;
                Console.Out.Write(_format(level, message, e));
                Console.Out.Flush();
                Console.ResetColor();
            }
        }

        private string WriteLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "VRB";
                case LogLevel.Debug:
                    return "DBG";
                case LogLevel.Information:
                    return "INF";
                case LogLevel.Warning:
                    return "WRN";
                case LogLevel.Error:
                    return "ERR";
                case LogLevel.Critical:
                    return "CRI";
                default:
                    return string.Empty;
            }
        }

        private (ConsoleColor Foreground, ConsoleColor Background) GetColors(LogLevel level)
        {
            ConsoleColor foreground;
            ConsoleColor background;

            switch (level)
            {
                case LogLevel.Trace:
                    foreground = _consoleTheme.VerboseForegroundColor;
                    background = _consoleTheme.VerboseBackgroundColor;
                    break;
                case LogLevel.Debug:
                    foreground = _consoleTheme.DebugForegroundColor;
                    background = _consoleTheme.DebugBackgroundColor;
                    break;
                case LogLevel.Information:
                    foreground = _consoleTheme.InformationVerboseForegroundColor;
                    background = _consoleTheme.InformationBackgroundColor;
                    break;
                case LogLevel.Warning:
                    foreground = _consoleTheme.WarningForegroundColor;
                    background = _consoleTheme.WarningBackgroundColor;
                    break;
                case LogLevel.Error:
                    foreground = _consoleTheme.ErrorForegroundColor;
                    background = _consoleTheme.ErrorBackgroundColor;
                    break;
                case LogLevel.Critical:
                    foreground = _consoleTheme.CriticalForegroundColor;
                    background = _consoleTheme.CriticalBackgroundColor;
                    break;
                default:
                    foreground = ConsoleColor.White;
                    background = ConsoleColor.Blue;
                    break;
            }

            return (foreground, background);
        }
    }
}
