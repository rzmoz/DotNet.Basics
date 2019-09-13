using System.Drawing;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class ConsoleTheme
    {
        public static ConsoleTheme Default { get; } = new ConsoleTheme
        {
            Verbose = new ConsoleFormat(Color.DarkSlateGray, Color.Empty, Color.DarkGray),
            Debug = new ConsoleFormat(Color.DarkCyan, Color.Empty, Color.DarkGray),
            Information = new ConsoleFormat(Color.White, Color.Empty, Color.Cyan),
            Success = new ConsoleFormat(Color.Green, Color.Empty, Color.White),
            Warning = new ConsoleFormat(Color.Yellow, Color.Empty,Color.Black, Color.DarkOrange),
            Error = new ConsoleFormat(Color.Red, Color.Empty, Color.Red, Color.Black),
            Critical = new ConsoleFormat(Color.White, Color.DarkRed, Color.White, Color.Black)
        };

        public ConsoleFormat Verbose { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Debug { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Information { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Success { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Warning { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Error { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Critical { get; set; } = ConsoleFormat.Empty;

        public ConsoleFormat Get(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return Verbose;
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Info:
                    return Information;
                case LogLevel.Success:
                    return Success;
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
