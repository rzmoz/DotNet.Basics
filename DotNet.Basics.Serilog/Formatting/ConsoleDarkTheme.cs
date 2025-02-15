using System;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleDarkTheme() : ConsoleTheme(GetForegroundColors(), GetForegroundHighlightColors())
    {
        private static ConsoleColorSet GetForegroundColors()
        {
            return new ConsoleColorSet(ConsoleColor.Green)
            {
                { LogEventLevel.Verbose, ConsoleColor.DarkGray},
                { LogEventLevel.Debug, ConsoleColor.DarkCyan},
                { LogEventLevel.Information, ConsoleColor.White },
                { LogEventLevel.Warning, ConsoleColor.DarkYellow},
                { LogEventLevel.Error, ConsoleColor.Red},
                { LogEventLevel.Fatal, ConsoleColor.White,ConsoleColor.DarkRed}
        };
        }
        private static ConsoleColorSet GetForegroundHighlightColors()
        {
            return new ConsoleColorSet(ConsoleColor.Black, ConsoleColor.DarkGreen)
            {
                { LogEventLevel.Verbose, ConsoleColor.Black, ConsoleColor.DarkGray},
                { LogEventLevel.Debug, ConsoleColor.Black, ConsoleColor.DarkCyan},
                { LogEventLevel.Information, ConsoleColor.Black, ConsoleColor.Gray},
                { LogEventLevel.Warning, ConsoleColor.Black, ConsoleColor.Yellow},
                { LogEventLevel.Error, ConsoleColor.Black, ConsoleColor.DarkRed},
                { LogEventLevel.Fatal, ConsoleColor.Black, ConsoleColor.Red}
            };
        }
    }
}
