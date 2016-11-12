using System.Collections.Generic;
using System.Linq;
using Autofac;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tests.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Ioc
{
    public class IocExtensionsTests
    {
        private readonly IocBuilder _builder;

        public IocExtensionsTests()
        {
            _builder = new IocBuilder();
        }
        [Fact]
        public void RegisterPipelineSteps_TypeBasedRegistrations_AllDescendantTypesAreRegistered_TypeIsResolved()
        {
            _builder.RegisterPipelineSteps();

            var resolved = new List<object>
            {
                _builder.Container.Resolve<AncestorStep>(),
                _builder.Container.Resolve<DescendantStep>(),
                _builder.Container.Resolve<AddIssueStep>()
            };

            resolved.Count.Should().Be(3);

            resolved.Any(r => r == null).Should().BeFalse();
        }
    }
}
