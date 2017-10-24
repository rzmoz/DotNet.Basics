using Autofac;

namespace DotNet.Basics.Extensions.Autofac
{
    public interface IAutofacRegistrations
    {
        void RegisterIn(ContainerBuilder builder);
    }
}
