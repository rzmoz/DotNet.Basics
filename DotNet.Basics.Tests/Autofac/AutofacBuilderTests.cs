using Autofac;
using DotNet.Basics.Autofac;
using DotNet.Basics.Tests.Autofac.TestHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Autofac
{
    public class AutofacBuilderTests
    {
        [Fact]
        public void AddRegistrations_Add_RegistrationsAreAdded()
        {
            var builder= new AutofacBuilder();
            builder.AddRegistrations(new MyAutofacRegistrations());

            var container = builder.Container;

            var resolved = container.Resolve<IMyType>();
            resolved.Should().BeOfType<MyType1>();
        }
    }
}
