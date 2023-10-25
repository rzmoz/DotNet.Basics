using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DotNet.Basics.Serilog
{
    public static class SerilogInitExtensions
    {
        public static IServiceCollection AddSerilogAndClearOtherProviders(this IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddSerilog();
            });
            return services;
        }
    }
}
