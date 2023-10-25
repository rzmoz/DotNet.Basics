using SimpleInjector;

namespace DotNet.Basics.IoC
{
    public interface IContainerConfiguration
    {
        void Configure(Container container);
    }
}
