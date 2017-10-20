using System;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace DotNet.Basics.Autofac
{
    public class AutofacBuilder : ContainerBuilder
    {
        private readonly Lazy<IContainer> _getContainer;

        public AutofacBuilder(bool resolveConcreteTypesNotAlreadyRegistered = true)
        {
            if (resolveConcreteTypesNotAlreadyRegistered)
                this.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            _getContainer = new Lazy<IContainer>(() => Build());
        }

        public IContainer Container => _getContainer.Value;

        public void Register(params IAutofacRegistrations[] registrations)
        {
            foreach (IAutofacRegistrations t in registrations)
                t.RegisterIn(this);
        }
    }
}
