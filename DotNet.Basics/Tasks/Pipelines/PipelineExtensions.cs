using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Basics.Reflection;

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
    }
}