﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Pipelines
{
    public static class PipelineExtensions
    {
        public static void AddPipelines(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool> where = null)
        {
            var pipelines = assemblies.SelectMany(a => GetPipelineTypes(a, where)).ToArray();
            services.AddPipelines(pipelines);
        }

        public static void AddPipelines(this IServiceCollection services, params IEnumerable<Type> pipelineTypes)
        {
            foreach (var pipelineStepType in pipelineTypes)
                services.AddTransient(pipelineStepType);
        }

        public static void AddPipelineSteps(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool> where = null)
        {
            var steps = assemblies.SelectMany(a => GetPipelineStepTypes(a, where)).ToArray();
            services.AddPipelineSteps(steps);
        }

        public static void AddPipelineSteps(this IServiceCollection services, params IEnumerable<Type> pipelineStepTypes)
        {
            foreach (var pipelineStepType in pipelineStepTypes)
                services.AddTransient(pipelineStepType);
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