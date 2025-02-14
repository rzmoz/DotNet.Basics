using Serilog.Configuration;
using Serilog.Events;
using Serilog;
using System;
using DotNet.Basics.Serilog.Diagnostics;
using DotNet.Basics.Serilog.Sinks;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Serilog
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDiagnosticsWithSerilogDevConsole(this IServiceCollection services, bool isADO = false, bool verbose = true)
        {
            return services.AddDiagnosticsWithSerilog(config =>
            {
                config.MinimumLevel.Is(verbose ? LogEventLevel.Verbose : LogEventLevel.Debug);
                return config.WriteTo.DevConsole(isADO, verbose);
            });
        }

        public static IServiceCollection AddDiagnosticsWithSerilog(this IServiceCollection services, Func<LoggerConfiguration, LoggerConfiguration> config)
        {
            return services.AddDiagnosticsWithSerilog(config, 1.Minutes());
        }
        public static IServiceCollection AddDiagnosticsWithSerilog(this IServiceCollection services, Func<LoggerConfiguration, LoggerConfiguration> config, TimeSpan longRunningOperationsPingInterval)
        {
            Log.Logger = config(new LoggerConfiguration()).CreateLogger();
            services.AddSingleton<ILoog>(new Loog().WithLogTarget(new SerilogLoogTarget()));
            services.AddSingleton(s =>
            {
                var logger = s.GetService<ILoog>()!;
                return new LongRunningOperations(logger, longRunningOperationsPingInterval);
            });
            return services;
        }

        public static LoggerConfiguration DevConsole(this LoggerSinkConfiguration sinkConfiguration, bool isADO = false, bool verbose = true)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            return sinkConfiguration.Sink(new DevConsoleSink(isADO), verbose ? LogEventLevel.Verbose : LogEventLevel.Debug);
        }
    }
}
