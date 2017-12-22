using DotNet.Basics.DependencyInjection;
using DotNet.Basics.Tests.Extensions.DependencyInjection.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Extensions.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddRegistrations_Add_RegistrationsAreAdded()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRegistrations(new MyIocRegistrations());

            var provider = serviceCollection.BuildServiceProvider();

            var resolved = provider.GetService<IMyType>();
            resolved.Should().BeOfType<MyType1>();
        }
    }
}
