using Autofac;

namespace DotNet.Standard.Extensions.DependencyInjection
{
    public interface IRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
