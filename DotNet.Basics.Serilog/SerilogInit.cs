using System;
using Serilog;
using Serilog.Events;

namespace DotNet.Basics.Serilog
{
    public static class SerilogInit
    {
        public static ILogger SetGlobalLogger()
        {
            return SetGlobalLogger(GetDefaultConfiguration());
        }
        public static ILogger SetGlobalLogger(LoggerConfiguration logConfig)
        {
            Log.Logger = logConfig.CreateLogger();
            return Log.Logger;
        }
        public static ILogger SetGlobalLogger(Action<LoggerConfiguration> additionalLogConfig)
        {
            var loggerConfig = GetDefaultConfiguration();
            additionalLogConfig(loggerConfig);
            return SetGlobalLogger(loggerConfig);
        }
        public static ILogger GetLogger(Func<LoggerConfiguration, LoggerConfiguration> configureLogger)
        {
            return configureLogger(new LoggerConfiguration()).CreateLogger();
        }

        public static LoggerConfiguration GetDefaultConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Verbose()
                .WriteTo.Console();
        }
    }
}
