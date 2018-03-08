using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.DependencyInjection
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddRegistrations_Add_RegistrationsAreAdded()
        {
            var serviceCollection = new ServiceCollection();

            var registrations = new MyServiceCollectionRegistrations();

            registrations.RegisterIn(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            var resolved = provider.GetService(typeof(IMyType));
            resolved.Should().BeOfType<MyType1>();
        }
    }
}
