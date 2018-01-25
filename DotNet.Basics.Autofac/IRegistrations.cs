using Autofac;

namespace DotNet.Basics.Autofac
{
    public interface IRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
