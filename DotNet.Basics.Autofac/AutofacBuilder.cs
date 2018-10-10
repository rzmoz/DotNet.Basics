using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Autofac
{
    public class AutofacBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly Lazy<IContainer> _getContainer;

        public AutofacBuilder(bool resolveConcreteTypesNotAlreadyRegistered = false)
        {
            _containerBuilder = new ContainerBuilder();
            if (resolveConcreteTypesNotAlreadyRegistered)
                _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            _getContainer = new Lazy<IContainer>(() => _containerBuilder.Build());
        }

        public IContainer Container => _getContainer.Value;
        public IServiceProvider ServiceProvider => new AutofacServiceProvider(Container);

        public AutofacBuilder WithServices(params IServiceCollection[] serviceCollections)
        {
            foreach (var serviceCollection in serviceCollections)
                _containerBuilder.Populate(serviceCollection);
            return this;
        }

        public AutofacBuilder WithRegistrations(params IAutofacRegistrations[] registrations)
        {
            foreach (var r in registrations)
                r.RegisterIn(_containerBuilder);
            return this;
        }

        public AutofacBuilder WithRegistrations(params Assembly[] assemblies)
        {
            WithRegistrations(assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IAutofacRegistrations).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<IAutofacRegistrations>().ToArray());
            return this;
        }
    }
}