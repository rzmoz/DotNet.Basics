using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Reflection;

namespace DotNet.Basics.Cli
{
    public static class CliExtensions
    {
        /// <summary>
        /// Adds all cli commands and settings to DI
        /// </summary>
        /// <typeparam name="T">Any type in assembly to scan in</typeparam>                
        public static IServiceCollection AddCommandsAndSettings<T>(this IServiceCollection services)
        {
            return services.AddCommandsAndSettings(typeof(T).Assembly);
        }
        /// <summary>
        /// Adds all cli commands and settings to DI
        /// </summary>
        /// <typeparam name="T">Any type in assembly to scan in</typeparam>        
        public static IServiceCollection AddCommandsAndSettings(this IServiceCollection services, params IEnumerable<Assembly> assemblies)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract).AssignableTo(typeof(ICommand<>)))
                    .AsSelf()
                    .WithTransientLifetime()
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract).AssignableTo(typeof(CommandSettings)))
                    .AsSelf()
                    .WithSingletonLifetime();
            });
            return services;
        }
    }
}
