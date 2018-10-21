using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;

namespace DotNet.Basics.Autofac
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder EnableResolvingIfTypesNotRegistered(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            return containerBuilder;
        }

        public static IServiceProvider BuildServiceProvider(this ContainerBuilder containerBuilder, ContainerBuildOptions options = ContainerBuildOptions.None)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            var container = containerBuilder.Build(options);
            return container.BuildServiceProvider();
        }
        public static IServiceProvider BuildServiceProvider(this IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return new AutofacServiceProvider(container);
        }
    }
}
