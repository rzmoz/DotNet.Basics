using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Time;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DotNet.Basics.NLog
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNLogging(this IServiceCollection services, Action<LoggingConfiguration> configureNLog = null, TimeSource timeSource = null, LogLevel minimumLogLevel = LogLevel.Trace)
        {
            new LoggingConfiguration().ConfigureNLog(configureNLog);
            services.TryAddSingleton<ILogger, Logger<ILogger>>();
            services.AddLogging(builder => builder.AddNLog().SetMinimumLevel(minimumLogLevel));
        }
    }
}
