using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Autofac
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, params IConfigureServices[] configureServices)
        {
            foreach (var configureService in configureServices)
            {
                configureService.Configure(services);
            }
            return services;
        }

        public static IServiceCollection AddServices<T>(this IServiceCollection services) where T : IConfigureServices
        {
            var configureService = Activator.CreateInstance<T>();
            configureService.Configure(services);
            return services;
        }
    }
}
