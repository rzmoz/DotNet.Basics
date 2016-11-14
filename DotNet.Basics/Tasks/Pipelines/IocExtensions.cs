using System;
using System.Linq;
using System.Reflection;
using Autofac;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tasks.Pipelines
{
    public static class IocExtensions
    {
        public static void RegisterPipelineSteps(this IocBuilder builder, params Assembly[] assemblies)
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
        
        private static void RegisterType(this Type type, IocBuilder builder, Type pipelineStepType)
        {
            if (type.IsAbstract)
                return;
            try
            {
                if (type.IsGenericType)
                    builder.RegisterGeneric(type);
                else
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
            }
            catch (Exception)
            {
                DebugOut.WriteLine($"Failed to register {type} in pipeline step: {pipelineStepType}");
                throw;
            }
        }
    }
}
