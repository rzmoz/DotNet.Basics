using System.Drawing;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class ConsoleTheme
    {
        public static ConsoleTheme Default { get; } = new ConsoleTheme
        {
            Verbose = new ConsoleFormat(Color.DarkSlateGray, Color.Empty, Color.DarkGray),
            Debug = new ConsoleFormat(Color.DarkCyan, Color.Empty, Color.DarkGray),
            Information = new ConsoleFormat(Color.White, Color.Empty, Color.Cyan),
            Warning = new ConsoleFormat(Color.Yellow, Color.Empty, Color.DarkOrange),
            Error = new ConsoleFormat(Color.Red, Color.Empty, Color.Red, Color.Black),
            Critical = new ConsoleFormat(Color.White, Color.DarkRed, Color.White, Color.Black)
        };

        public ConsoleFormat Verbose { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Debug { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Information { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Warning { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Error { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Critical { get; set; } = ConsoleFormat.Empty;

        public ConsoleFormat Get(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return Verbose;
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Information:
                    return Information;
                case LogLevel.Warning:
                    return Warning;
                case LogLevel.Error:
                    return Error;
                case LogLevel.Critical:
                    return Critical;
                default:
                    return ConsoleFormat.Empty;
            }
        }
    }
}
