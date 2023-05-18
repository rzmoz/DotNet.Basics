using SimpleInjector;

namespace DotNet.Basics.DependencyInjection
{
    public static class ContainerConfigurationExtensions
    {
        public static Container BuildContainer(this IContainerConfiguration containerConfiguration)
        {
            var container = new Container();
            containerConfiguration.Configure(container);
            return container;
        }
    }
}
