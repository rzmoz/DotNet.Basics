using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tasks.Pipelines
{
    public static class IocExtensions
    {
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
           RegisterPipelineSteps(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };
            return builder.RegisterAll<IPipelineStep>(assemblies);
        }
    }
}
