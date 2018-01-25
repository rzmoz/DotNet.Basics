using Autofac;

namespace DotNet.Basics.Extensions.DependencyInjection
{
    public interface IRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
