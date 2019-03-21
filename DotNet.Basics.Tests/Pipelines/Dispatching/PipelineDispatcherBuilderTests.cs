using DotNet.Basics.Pipelines.Dispatching;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines.Dispatching
{
    public class PipelineDispatcherBuilderTests
    {
        [Fact]
        public void Build_BuildDispatcher_DispatcherIsBuild()
        {
            var dispatcher = new PipelineDispatcherBuilder()
                .WithRootNamespace(typeof(PipelineDispatcherBuilderTests).Namespace)
                .Build(typeof(PipelineDispatcherBuilderTests).Assembly);

            dispatcher.Pipelines.Count.Should().Be(1);
        }
    }
}
