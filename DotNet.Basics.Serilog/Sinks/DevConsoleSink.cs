

using System;
using System.IO;
using DotNet.Basics.Serilog.Formatting;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace DotNet.Basics.Serilog.Sinks
{
    public class DevConsoleSink(bool isADO) : ILogEventSink
    {
        private static readonly object _messageLock = new();

        private readonly ITextFormatter _formatter = isADO ? new AdoPipelinesTextFormatter() : new DevConsoleFormatter();


        public void Emit(LogEvent logEvent)
        {
            var output = GetOutStream(logEvent.Level);

            lock (_messageLock)
            {
                _formatter.Format(logEvent, output);
                output.Flush();
            }
        }

        private TextWriter GetOutStream(LogEventLevel lvl)
        {
            return lvl < LogEventLevel.Error ? Console.Out : Console.Error;
        }
    }
}
