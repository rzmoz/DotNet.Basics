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
                { LogEventLevel.Debug, ConsoleColor.DarkGray},
                { LogEventLevel.Information, ConsoleColor.Cyan},
                { LogEventLevel.Warning, ConsoleColor.Yellow},
                { LogEventLevel.Error, ConsoleColor.DarkRed},
                { LogEventLevel.Fatal, ConsoleColor.Yellow,ConsoleColor.Red}
        };
        }
        private static ConsoleColorSet GetForegroundHighlightColors()
        {
            return new ConsoleColorSet(ConsoleColor.DarkGreen)
            {
                { LogEventLevel.Verbose, ConsoleColor.DarkYellow},
                { LogEventLevel.Debug, ConsoleColor.DarkYellow},
                { LogEventLevel.Information, ConsoleColor.White},
                { LogEventLevel.Warning, ConsoleColor.DarkYellow},
                { LogEventLevel.Error, ConsoleColor.Red},
                { LogEventLevel.Fatal, ConsoleColor.Black, ConsoleColor.Red}
            };
        }
    }
}
