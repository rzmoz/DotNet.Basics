using Autofac;
using DotNet.Basics.Extensions.DependencyInjection;
using DotNet.Basics.Tests.Extensions.DepencyInjections.TestHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Extensions.DepencyInjections
{
    public class AutofacBuilderTests
    {
        [Fact]
        public void AddRegistrations_Add_RegistrationsAreAdded()
        {
            var builder= new AutofacBuilder();
            builder.AddRegistrations(new MyIocRegistrations());

            var container = builder.Container;

            var resolved = container.Resolve<IMyType>();
            resolved.Should().BeOfType<MyType1>();
        }
    }
}
