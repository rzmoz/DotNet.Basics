using System;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleDarkTheme() : ConsoleTheme(GetForegroundColors(), GetForegroundHighlightColors())
    {
        private static ConsoleColorSet GetForegroundColors()
        {
            return new ConsoleColorSet(ConsoleColor.DarkGreen)
            {
                { LogEventLevel.Verbose, ConsoleColor.DarkGray},
                { LogEventLevel.Debug, ConsoleColor.DarkCyan},
                { LogEventLevel.Information, ConsoleColor.Gray},
                { LogEventLevel.Warning, ConsoleColor.DarkYellow},
                { LogEventLevel.Error, ConsoleColor.DarkRed},
                { LogEventLevel.Fatal, ConsoleColor.Black,ConsoleColor.Red}
        };
        }
        private static ConsoleColorSet GetForegroundHighlightColors()
        {
            return new ConsoleColorSet(ConsoleColor.Green)
            {
                { LogEventLevel.Verbose, ConsoleColor.Blue},
                { LogEventLevel.Debug, ConsoleColor.Cyan},
                { LogEventLevel.Information, ConsoleColor.White },
                { LogEventLevel.Warning, ConsoleColor.Yellow},
                { LogEventLevel.Error, ConsoleColor.Red},
                { LogEventLevel.Fatal, ConsoleColor.Yellow, ConsoleColor.Red}
            };
        }
    }
}
