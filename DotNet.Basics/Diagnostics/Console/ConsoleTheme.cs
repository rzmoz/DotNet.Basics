using System.Drawing;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Diagnostics.Console
{
    public class ConsoleTheme
    {
        public static ConsoleTheme Default { get; } = new ConsoleTheme
        {
            Raw = new ConsoleFormat(),
            Verbose = new ConsoleFormat(Color.DarkSlateGray, Color.Empty, Color.Gray),
            Debug = new ConsoleFormat(Color.DarkCyan, Color.Empty, Color.DarkTurquoise),
            Information = new ConsoleFormat(Color.White, Color.Empty, Color.Violet),
            Success = new ConsoleFormat(Color.Green, Color.Empty, Color.LightGreen),
            Warning = new ConsoleFormat(Color.DarkOrange, Color.Empty, Color.Yellow),
            Error = new ConsoleFormat(Color.Red, Color.Empty, Color.White, Color.DarkRed)
        };

        public ConsoleFormat Raw { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Verbose { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Debug { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Information { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Success { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Warning { get; set; } = ConsoleFormat.Empty;
        public ConsoleFormat Error { get; set; } = ConsoleFormat.Empty;

        public ConsoleFormat Get(LogLevel level) =>
            level switch
            {

                LogLevel.Raw => Raw,
                LogLevel.Verbose => Verbose,
                LogLevel.Debug => Debug,
                LogLevel.Info => Information,
                LogLevel.Success => Success,
                LogLevel.Warning => Warning,
                LogLevel.Error => Error,
                _ => ConsoleFormat.Empty
            };
    }
}
