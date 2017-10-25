using System;
using System.Linq;
using System.Reflection;
using Autofac;
using DotNet.Basics.Extensions.Autofac;

namespace DotNet.Basics.Pipelines
{
    public static class PipelineExtensions
    {
        public static void RegisterPipelineSteps<T>(this AutofacBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterPipelineSteps(assemblies.Concat(new[] { typeof(T).Assembly }).ToArray());
        }

        public static void RegisterPipelineSteps(this AutofacBuilder builder, params Assembly[] assemblies)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };

            builder.RegisterGeneric(typeof(Pipeline<>)).ExternallyOwned();
            //get all steps
            var pipelineSteps = assemblies.SelectMany(a =>
                a.GetTypes().Where(t => t.BaseType != null &&
                                        t.BaseType.IsGenericType &&
                                        t.BaseType.GetGenericTypeDefinition() == typeof(PipelineStep<>))).ToList();

            foreach (var pipelineStep in pipelineSteps)
                pipelineStep.RegisterType(builder, pipelineStep);
        }

        private static void RegisterType(this Type type, AutofacBuilder builder, Type pipelineStepType)
        {
            if (type.IsAbstract)
                return;

            if (type.IsGenericType)
                builder.RegisterGeneric(type);
            else
                builder.RegisterType(type).AsSelf();
        }
    }
}