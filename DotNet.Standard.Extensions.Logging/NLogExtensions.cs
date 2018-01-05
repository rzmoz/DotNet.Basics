using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNet.Standard.Extensions.Logging
{
    public static class NLogExtensions
    {
        public static void AddNLogging(this IServiceCollection services, LogLevel logLevel = LogLevel.Trace)
        {
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(builder => builder.SetMinimumLevel(logLevel));
        }
    }
}
