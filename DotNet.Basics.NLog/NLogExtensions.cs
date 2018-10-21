using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;

namespace DotNet.Basics.NLog
{
    public static class NLogExtensions
    {
        public static void AddNLogging(this IServiceCollection services, Action<NLogConfigurator> addTargets = null, LoggingConfiguration config = null, LogLevel minimumLogLevel = LogLevel.Trace)
        {
            var nLogConfigurator = new NLogConfigurator(config);
            addTargets?.Invoke(nLogConfigurator);
            nLogConfigurator.SaveToGlobal();

            services.TryAddSingleton<ILogger, Logger<ILogger>>();
            services.AddLogging(builder => builder.AddNLog().SetMinimumLevel(minimumLogLevel));
        }
    }
}
