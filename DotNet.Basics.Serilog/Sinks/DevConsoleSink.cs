

using System;
using Serilog.Core;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Sinks
{
    public class DevConsoleSink(bool isADO) : ILogEventSink
    {
        private static readonly object _messageLock = new();
        private readonly Func<LogEvent, string> _renderMessage = le => isADO ? RenderAdoMessage(le) : le.RenderMessage();
        private readonly Func<LogEventLevel, ConsoleColor> _getConsoleColor = le => isADO ? ConsoleColor.White : GetColor(le);


        private static string RenderAdoMessage(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                    return $"##[debug]{logEvent.RenderMessage()}";
                case LogEventLevel.Information:
                    return $"##[command]{logEvent.RenderMessage()}";
                case LogEventLevel.Warning:
                    return $"##[warning]{logEvent.RenderMessage()}";
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return $"##[error]{logEvent.RenderMessage()}";
                default:
                    return logEvent.RenderMessage();
            }
        }

        public void Emit(LogEvent logEvent)
        {
            var msg = _renderMessage(logEvent);

            lock (_messageLock)
            {
                Console.ForegroundColor = _getConsoleColor(logEvent.Level);
                if (logEvent.Level < LogEventLevel.Error)
                    Console.WriteLine(msg);
                else
                    Console.Error.WriteLine(msg);
                Console.ResetColor();
            }
        }

        private static ConsoleColor GetColor(LogEventLevel lvl)
        {
            switch (lvl)
            {
                case LogEventLevel.Verbose:
                    return ConsoleColor.DarkGray;
                case LogEventLevel.Debug:
                    return ConsoleColor.Gray;
                case LogEventLevel.Information:
                    return ConsoleColor.Cyan;
                case LogEventLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogEventLevel.Error:
                    return ConsoleColor.Red;
                case LogEventLevel.Fatal:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
