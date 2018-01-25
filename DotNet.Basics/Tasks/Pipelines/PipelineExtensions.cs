using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.Tasks.Pipelines
{
    public static class PipelineExtensions
    {
        public static IEnumerable<Type> GetPipelineTypes(this Assembly assembly)
        {
            return assembly.GetTypesOf(typeof(Pipeline<>));
        }

        public static IEnumerable<Type> GetPipelineStepTypes(this Assembly assembly)
        {
            return assembly.GetTypesOf(typeof(PipelineStep<>));
        }

        public static IEnumerable<Type> GetTypesOf(this Assembly assembly, Type typeOf)
        {
            return assembly.GetTypes().Where(t => t.BaseType != null &&
                                                  t.IsAbstract == false &&
                                                  t.BaseType.IsGenericType &&
                                                  t.BaseType.GetGenericTypeDefinition() == typeOf);
        }
    }
}