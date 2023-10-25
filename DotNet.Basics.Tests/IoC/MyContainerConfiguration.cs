using DotNet.Basics.IoC;
using SimpleInjector;

namespace DotNet.Basics.Tests.DependencyInjection
{
    public class MyContainerConfiguration : IContainerConfiguration
    {
        public void Configure(Container container)
        {
            container.Register<IMyType, MyType1>();
        }
    }
}
