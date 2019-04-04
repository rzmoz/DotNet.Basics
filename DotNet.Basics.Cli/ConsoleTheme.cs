using System.Drawing;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class ConsoleTheme
    {
        public static ConsoleTheme Default { get; } = new ConsoleTheme(
            new ConsoleFormat(new AnsiForegroundColor(Color.DarkGray), AnsiBackgroundColor.Empty, new AnsiForegroundColor(Color.DarkCyan)),
            new ConsoleFormat(new AnsiForegroundColor(Color.DarkCyan), AnsiBackgroundColor.Empty, new AnsiForegroundColor(Color.White)),
            new ConsoleFormat(new AnsiForegroundColor(Color.White), AnsiBackgroundColor.Empty, new AnsiForegroundColor(Color.Cyan)),
            new ConsoleFormat(new AnsiForegroundColor(Color.Yellow), AnsiBackgroundColor.Empty, new AnsiForegroundColor(Color.Black)),
            new ConsoleFormat(new AnsiForegroundColor(Color.Red), AnsiBackgroundColor.Empty, new AnsiForegroundColor(Color.Blue)),
            new ConsoleFormat(new AnsiForegroundColor(Color.White), new AnsiBackgroundColor(Color.DarkRed), new AnsiForegroundColor(Color.DarkCyan)));

        public ConsoleTheme()
        {
        }
        public ConsoleTheme(ConsoleFormat verbose, ConsoleFormat debug, ConsoleFormat information, ConsoleFormat warning, ConsoleFormat error, ConsoleFormat critical)
        {
            Verbose = verbose;
            Debug = debug;
            Information = information;
            Warning = warning;
            Error = error;
            Critical = critical;
        }

        public ConsoleFormat Verbose { get; } = ConsoleFormat.Empty;
        public ConsoleFormat Debug { get; } = ConsoleFormat.Empty;
        public ConsoleFormat Information { get; } = ConsoleFormat.Empty;
        public ConsoleFormat Warning { get; } = ConsoleFormat.Empty;
        public ConsoleFormat Error { get; } = ConsoleFormat.Empty;
        public ConsoleFormat Critical { get; } = ConsoleFormat.Empty;

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
