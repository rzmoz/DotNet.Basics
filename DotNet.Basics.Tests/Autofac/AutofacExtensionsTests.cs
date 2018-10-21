using Autofac;
using DotNet.Basics.Autofac;
using DotNet.Basics.Tests.Autofac.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Autofac
{
    public class AutofacExtensionsTests
    {
        [Fact]
        public void BuildAutofacServiceProvider_Build_InitializeAutofacProvider()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddTransient<TypeWithValue>();

            //act
            var serviceProvider = services.BuildAutofacServiceProvider();

            //assert
            //resolve registered Type
            var myType = serviceProvider.GetService<TypeWithValue>();
            myType.Should().BeOfType<TypeWithValue>();
            //resolve unregistered Type
            var unregisteredType = serviceProvider.GetService<AutofacExtensionsTests>();
            unregisteredType.Should().BeNull();
        }

        [Fact]
        public void EnableResolvingIfTypesNotRegistered_UnRegisteredTypes_TypesAreResolved()
        {
            //act
            var builder = new ContainerBuilder().EnableResolvingIfTypesNotRegistered();

            var serviceProvider = builder.BuildServiceProvider();
            var myType = serviceProvider.GetService<TypeWithValue>();
            myType.Should().BeOfType<TypeWithValue>();
        }
    }
}
