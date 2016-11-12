using System;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;

namespace DotNet.Basics.Ioc
{
    public static class IocExtensions
    {
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
           RegisterAll<T>(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };
            return builder.RegisterAll(typeof(T), assemblies);
        }

        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
           RegisterAll(this ContainerBuilder builder, Type type, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };
            return builder.RegisterAssemblyTypes(assemblies)
                .Where(type.IsAssignableFrom)
                .ExternallyOwned();
        }
    }
}
