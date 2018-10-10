using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRegistrations(this IServiceCollection services, params IRegistrations[] registrations)
        {
            foreach (var registration in registrations)
                registration.RegisterIn(services);
        }

        public static void AddRegistrations(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddRegistrations(assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IRegistrations).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<IRegistrations>().ToArray());
        }
    }
}
