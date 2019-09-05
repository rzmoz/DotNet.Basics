using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public static class ConsoleFormatExtensions
    {
        public static string ToOutputString(this LogLevel level)
        {
            return level < LogLevel.None
                ? $"{level.ToName().ToUpperInvariant()}"
                : string.Empty;
        }
    }
}