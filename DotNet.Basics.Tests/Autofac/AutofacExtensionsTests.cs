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
        private readonly ContainerBuilder _builder;

        public AutofacExtensionsTests()
        {
            _builder = new ContainerBuilder();
        }

        [Fact]
        public void EnableResolvingIfTypesNotRegistered_UnRegisteredTypes_TypesAreResolved()
        {
            //act
            _builder.EnableResolvingIfTypesNotRegistered();

            var serviceProvider = _builder.BuildServiceProvider();
            var myType = serviceProvider.GetService<TypeWithValue>();
            myType.Should().BeOfType<TypeWithValue>();
        }
    }
}
