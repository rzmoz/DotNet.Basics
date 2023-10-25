using DotNet.Basics.IoC;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.DependencyInjection
{
    public class ContainerConfigurationExtensionsTests
    {
        [Fact]
        public void BuildContainer_Configure_InitializeServiceProvider()
        {
            //arrange
            var myRegistrations = new MyContainerConfiguration();

            //act
            var container = myRegistrations.BuildContainer();

            //assert
            //resolve registered Type
            var myType = container.GetService<IMyType>();
            myType.Should().BeOfType<MyType1>();
            //resolve unregistered Type
            var unregisteredType = container.GetService<MyType1>();//only registered as interface
            unregisteredType.Should().BeNull();
        }
    }
}
