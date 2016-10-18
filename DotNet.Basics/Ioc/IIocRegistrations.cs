using Autofac;

namespace DotNet.Basics.Ioc
{
    public interface IIocRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
