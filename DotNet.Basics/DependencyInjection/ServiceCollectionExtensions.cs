using System;
using DotNet.Basics.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, params IConfigureServices[] configureServices)
        {
            configureServices.ForEach(cf => cf.Configure(services));
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
