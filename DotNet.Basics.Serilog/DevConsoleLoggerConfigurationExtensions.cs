using Serilog.Configuration;
using Serilog.Events;
using Serilog;
using System;
using DotNet.Basics.Serilog.Sinks;

namespace DotNet.Basics.Serilog
{
    public static class DevConsoleLoggerConfigurationExtensions
    {
        public static LoggerConfiguration DevConsole(this LoggerSinkConfiguration sinkConfiguration, bool isDebug, bool isADO)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            return sinkConfiguration.Sink(new DevConsoleSink(isADO), isDebug ? LogEventLevel.Verbose : LogEventLevel.Information);
        }
    }
}
