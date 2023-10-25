using SimpleInjector;

namespace DotNet.Basics.IoC
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
