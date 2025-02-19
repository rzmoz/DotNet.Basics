using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Pipelines
{
    public static class PipelineExtensions
    {
        public static IServiceCollection AddPipelines(this IServiceCollection services,
            Func<Type, bool>? pipelineFilter = null, Func<Type, bool>? pipelineStepFilter = null)
        {
            return services.AddPipelines([Assembly.GetEntryAssembly() ?? throw new ApplicationException($"Entry assembly is null")], pipelineFilter, pipelineStepFilter);
        }
        public static IServiceCollection AddPipelines(this IServiceCollection services, Assembly[] assemblies, Func<Type, bool>? pipelineFilter = null, Func<Type, bool>? pipelineStepFilter = null)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract && (pipelineFilter?.Invoke(t) ?? true)).AssignableTo(typeof(Pipeline<>)))
                    .AsSelf()
                    .WithTransientLifetime()
                    .AddClasses(@class => @class.Where(t => !t.IsAbstract && (pipelineStepFilter?.Invoke(t) ?? true)).AssignableTo(typeof(PipelineStep<>)))
                    .AsSelf()
                    .WithTransientLifetime();
            });
            return services;
        }
    }
}