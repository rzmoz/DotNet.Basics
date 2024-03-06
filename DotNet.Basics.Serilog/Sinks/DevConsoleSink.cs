using System;
using Serilog.Core;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Sinks
{
    public class DevConsoleSink : ILogEventSink
    {
        private readonly Func<LogEvent, string> _renderMessage = le => le.RenderMessage();

        public DevConsoleSink(bool isADO)
        {
            if (isADO)
                _renderMessage = RenderAdoMessage;
        }

        private string RenderAdoMessage(LogEvent logEvent)
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

            if (logEvent.Level < LogEventLevel.Error)
                Console.WriteLine(msg);
            else
                Console.Error.WriteLine(msg);
        }
    }
}
