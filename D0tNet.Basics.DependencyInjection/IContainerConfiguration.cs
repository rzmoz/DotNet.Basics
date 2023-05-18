using SimpleInjector;

namespace DotNet.Basics.DependencyInjection
{
    public interface IContainerConfiguration
    {
        void Configure(Container container);
    }
}
