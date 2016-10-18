using Autofac;
using Autofac.Features.ResolveAnything;

namespace DotNet.Basics.Ioc
{
    public class IocBuilder : ContainerBuilder
    {
        public IocBuilder(bool resolveTypesNotAlreadyRegistered = true)
        {
            if (resolveTypesNotAlreadyRegistered)
                this.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}
