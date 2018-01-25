using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Extensions.DependencyInjection
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

        public void AddByContainerBuilder(Action<ContainerBuilder> registerServices)
        {
            registerServices?.Invoke(_containerBuilder);
        }

        public void AddByServiceCollection(Action<IServiceCollection> registerServices)
        {
            var serviceCollection = new ServiceCollection();
            registerServices?.Invoke(serviceCollection);
            _containerBuilder.Populate(serviceCollection);
        }

        public void AddServiceCollections(params IServiceCollection[] serviceCollections)
        {
            foreach (var serviceCollection in serviceCollections)
                _containerBuilder.Populate(serviceCollection);
        }

        public void AddRegistrations(params IRegistrations[] registrations)
        {
            foreach (var r in registrations)
                r.RegisterIn(_containerBuilder);
        }
    }
}