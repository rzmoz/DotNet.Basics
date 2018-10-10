using DotNet.Basics.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.DependencyInjection
{
    public class ServiceExtensionsTests
    {
        [Fact]
        public void AddRegistrations_Direct_RegistrationsAreAdded()
        {
            var services = new ServiceCollection();

            services.AddRegistrations(new MyServiceCollectionRegistrations());

            var provider = services.BuildServiceProvider();

            var resolved = provider.GetService(typeof(IMyType));
            resolved.Should().BeOfType<MyType1>();
        }

        [Fact]
        public void AddRegistrations_FromAssemblies_RegistrationsAreAdded()
        {
            var services = new ServiceCollection();

            services.AddRegistrations(typeof(DependencyInjectionTests).Assembly);

            var provider = services.BuildServiceProvider();

            var resolved = provider.GetService(typeof(IMyType));
            resolved.Should().BeOfType<MyType1>();
        }
    }
}
