using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class ConsoleDarkTheme() : ConsoleTheme(GetForegroundColors(), GetForegroundHighlightColors())
    {
        private static ConsoleColorSet GetForegroundColors()
        {
            return new ConsoleColorSet(ConsoleColor.DarkGreen)
            {
                { LogLevel.Trace, ConsoleColor.DarkGray},
                { LogLevel.Debug, ConsoleColor.DarkGray},
                { LogLevel.Information, ConsoleColor.Cyan},
                { LogLevel.Warning, ConsoleColor.Yellow},
                { LogLevel.Error, ConsoleColor.DarkRed},
                { LogLevel.Critical, ConsoleColor.Black,ConsoleColor.DarkRed}
        };
        }
        private static ConsoleColorSet GetForegroundHighlightColors()
        {
            return new ConsoleColorSet(ConsoleColor.Green)
            {
                { LogLevel.Trace, ConsoleColor.DarkYellow},
                { LogLevel.Debug, ConsoleColor.DarkYellow},
                { LogLevel.Information, ConsoleColor.White},
                { LogLevel.Warning, ConsoleColor.DarkYellow},
                { LogLevel.Error, ConsoleColor.Yellow},
                { LogLevel.Critical, ConsoleColor.Yellow, ConsoleColor.DarkRed}
            };
        }
    }
}
