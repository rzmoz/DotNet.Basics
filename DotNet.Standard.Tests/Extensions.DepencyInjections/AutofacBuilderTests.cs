using Autofac;
using DotNet.Standard.Extensions.DependencyInjection;
using DotNet.Standard.Tests.Extensions.DepencyInjections.TestHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Standard.Tests.Extensions.DepencyInjections
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
