using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Autofac
{
    public static class AutofacExtensions
    {
        public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services, Action<ContainerBuilder> autofacRegistrations = null, ContainerBuildOptions options = ContainerBuildOptions.None, bool resolveAnyConcreteTypeNotAlreadyRegisteredSource = false)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            var containerBuilder = new ContainerBuilder();
            if (resolveAnyConcreteTypeNotAlreadyRegisteredSource)
                containerBuilder.EnableResolvingIfTypesNotRegistered();
            autofacRegistrations?.Invoke(containerBuilder);
            containerBuilder.Populate(services);
            return containerBuilder.BuildServiceProvider(options);
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

        public static ContainerBuilder EnableResolvingIfTypesNotRegistered(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            return containerBuilder;
        }
    }
}
