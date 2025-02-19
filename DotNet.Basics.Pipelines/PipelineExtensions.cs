using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Pipelines
{
    public static class PipelineExtensions
    {
        public static IServiceCollection AddPipelines(this IServiceCollection services, Func<Type, bool> where = null)
        {
            var pipelines = Assembly.GetEntryAssembly().GetPipelineTypes(where);
            services.AddPipelines(pipelines);
            return services;
        }
        public static IServiceCollection AddPipelines(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool> where = null)
        {
            var pipelines = assemblies.SelectMany(a => GetPipelineTypes(a, where));
            services.AddPipelines(pipelines);
            return services;
        }

        public static IServiceCollection AddPipelines(this IServiceCollection services, IEnumerable<Type> pipelineTypes)
        {
            foreach (var pipelineStepType in pipelineTypes)
                services.AddTransient(pipelineStepType);
            return services;
        }

        public static IServiceCollection AddPipelineSteps(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool> where = null)
        {
            var steps = assemblies.SelectMany(a => GetPipelineStepTypes(a, where));
            services.AddPipelineSteps(steps);
            return services;
        }

        public static IServiceCollection AddPipelineSteps(this IServiceCollection services, IEnumerable<Type> pipelineStepTypes)
        {
            foreach (var pipelineStepType in pipelineStepTypes)
                services.AddTransient(pipelineStepType);
            return services;
        }

        public static IEnumerable<Type> GetPipelineTypes(this Assembly assembly, Func<Type, bool> where = null)
        {
            return assembly.GetTypesOf(typeof(Pipeline<>)).Where(t => !t.IsAbstract).Where(t => where?.Invoke(t) ?? true);
        }

        public static IEnumerable<Type> GetPipelineStepTypes(this Assembly assembly, Func<Type, bool> where = null)
        {
            return assembly.GetTypesOf(typeof(PipelineStep<>)).Where(t => !t.IsAbstract).Where(t => where?.Invoke(t) ?? true);
        }
    }
}