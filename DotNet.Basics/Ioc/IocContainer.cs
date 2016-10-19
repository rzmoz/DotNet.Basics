using System;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace DotNet.Basics.Ioc
{
    public class IocBuilder : ContainerBuilder
    {
        private readonly Lazy<IContainer> _getContainer;

        public IocBuilder(bool resolveTypesNotAlreadyRegistered = true)
        {
            if (resolveTypesNotAlreadyRegistered)
                this.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            _getContainer = new Lazy<IContainer>(() => Build());
        }

        public IContainer Container => _getContainer.Value;
    }
}
