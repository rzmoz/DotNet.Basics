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
        public static void AddPipelines(this IServiceCollection services, params Assembly[] assemblies)
        {
            var pipelines = assemblies.SelectMany(GetPipelineTypes).ToArray();
            services.AddPipelines(pipelines);
        }

        public static void AddPipelines(this IServiceCollection services, params Type[] pipelineTypes)
        {
            foreach (var pipelineStepType in pipelineTypes)
                services.AddTransient(pipelineStepType);
        }

        public static void AddPipelineSteps(this IServiceCollection services, params Assembly[] assemblies)
        {
            var steps = assemblies.SelectMany(GetPipelineStepTypes).ToArray();
            services.AddPipelineSteps(steps);
        }

        public static void AddPipelineSteps(this IServiceCollection services, params Type[] pipelineStepTypes)
        {
            foreach (var pipelineStepType in pipelineStepTypes)
                services.AddTransient(pipelineStepType);
        }

        public static IEnumerable<Type> GetPipelineTypes(this Assembly assembly)
        {
            return assembly.GetTypesOf(typeof(Pipeline<>));
        }

        public static IEnumerable<Type> GetPipelineStepTypes(this Assembly assembly)
        {
            return assembly.GetTypesOf(typeof(PipelineStep<>));
        }
    }
}