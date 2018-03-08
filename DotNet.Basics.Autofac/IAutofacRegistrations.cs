using Autofac;

namespace DotNet.Basics.Autofac
{
    public interface IAutofacRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
