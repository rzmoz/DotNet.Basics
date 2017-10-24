using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Extensions.Autofac
{
    public class AutofacBuilderServicesProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly IAutofacRegistrations[] _registrations;
        private readonly bool _resolveConcreteTypesNotAlreadyRegistered;

        public AutofacBuilderServicesProviderFactory(IAutofacRegistrations[] registrations, bool resolveConcreteTypesNotAlreadyRegistered = true)
        {
            _registrations = registrations;
            _resolveConcreteTypesNotAlreadyRegistered = resolveConcreteTypesNotAlreadyRegistered;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new AutofacBuilder(_resolveConcreteTypesNotAlreadyRegistered);

            builder.Populate(services);
            builder.Register(_registrations);

            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
