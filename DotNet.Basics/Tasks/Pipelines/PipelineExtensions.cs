using System;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Tasks.Pipelines
{
    public static class PipelineExtensions
    {
        public static void AddPipelines<T>(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddPipelines(typeof(T).Assembly.ToEnumerable(assemblies).ToArray());
        }

        public static void AddPipelines(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };

            services.AddTransient(typeof(Pipeline<>));
            //get all steps
            var pipelineSteps = assemblies.SelectMany(a =>
                a.GetTypes().Where(t => t.BaseType != null &&
                                        t.BaseType.IsGenericType &&
                                        t.BaseType.GetGenericTypeDefinition() == typeof(PipelineStep<>))).ToList();

            foreach (var pipelineStep in pipelineSteps)
                pipelineStep.RegisterType(services, pipelineStep);
        }

        private static void RegisterType(this Type type, IServiceCollection services, Type pipelineStepType)
        {
            if (type.IsAbstract)
                return;
            services.AddTransient(type);
        }
    }
}