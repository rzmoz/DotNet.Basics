using Serilog.Configuration;
using Serilog.Events;
using Serilog;
using System;
using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Serilog.Sinks;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Serilog
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDiagnosticsWithSerilogDevConsole(this IServiceCollection services, bool verbose = false, bool isADO = false)
        {
            return services.AddDiagnosticsWithSerilog(config =>
            {
                config.MinimumLevel.Is(verbose ? LogEventLevel.Verbose : LogEventLevel.Information);
                return config.WriteTo.DevConsole(verbose: verbose, isADO: isADO);
            }, verbose: verbose, isADO: isADO);
        }

        public static IServiceCollection AddDiagnosticsWithSerilog(this IServiceCollection services, Func<LoggerConfiguration, LoggerConfiguration> config, bool verbose, bool isADO)
        {
            return services.AddDiagnosticsWithSerilog(config, verbose: verbose, isADO: isADO, longRunningOperationsPingInterval: 1.Minutes());
        }
        public static IServiceCollection AddDiagnosticsWithSerilog(this IServiceCollection services, Func<LoggerConfiguration, LoggerConfiguration> config, bool verbose, bool isADO, TimeSpan longRunningOperationsPingInterval)
        {
            Log.Logger = config(new LoggerConfiguration()).CreateLogger();
            services.AddSingleton<ILoog>(new Loog().WithLogTarget(new SerilogLoogTarget(verbose: verbose, isADO: isADO)));
            services.AddSingleton(s =>
            {
                var logger = s.GetService<ILoog>()!;
                return new LongRunningOperations(logger, longRunningOperationsPingInterval);
            });
            return services;
        }

        public static LoggerConfiguration DevConsole(this LoggerSinkConfiguration sinkConfiguration, bool verbose = false, bool isADO = false)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            return sinkConfiguration.Sink(new DevConsoleSink(isADO: isADO), verbose ? LogEventLevel.Verbose : LogEventLevel.Information);
        }
    }
}
