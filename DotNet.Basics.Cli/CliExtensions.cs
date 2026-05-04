using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
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
        public static IServiceCollection AddCommandsAndSettings<T>(this IServiceCollection services, Func<Type, bool>? commandFilter = null, Func<Type, bool>? settingsFilter = null)
        {
            return services.AddCommandsAndSettings([typeof(T).Assembly]);
        }

        public static IServiceCollection AddCommandsAndSettings(this IServiceCollection services, IReadOnlyList<Assembly> assemblies, Func<Type, bool>? commandFilter = null, Func<Type, bool>? settingsFilter = null)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract && (commandFilter?.Invoke(t) ?? true)).AssignableTo(typeof(ICommand<>)))
                    .AsSelf()
                    .WithTransientLifetime()
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract && (settingsFilter?.Invoke(t) ?? true)).AssignableTo(typeof(CommandSettings)))
                    .AsSelf()
                    .WithSingletonLifetime();
            });
            return services;
        }
    }
}
