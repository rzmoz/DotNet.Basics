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
            {
                pipelineStep.RegisterType(builder);
                //resolve ctor params recursive
                pipelineStep.RegisterCtorParams(builder);
            }
        }

        private static void RegisterCtorParams(this Type type, IocBuilder builder)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var ctor in ctors)
            {
                foreach (var ctorParam in ctor.GetParameters())
                {
                    ctorParam.ParameterType.RegisterType(builder);
                    ctorParam.ParameterType.RegisterCtorParams(builder);
                }
            }
        }

        private static void RegisterType(this Type type, IocBuilder builder)
        {
            if (type.IsAbstract)
                return;
            if (type.IsGenericType)
                builder.RegisterGeneric(type);
            else
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
        }
    }
}
