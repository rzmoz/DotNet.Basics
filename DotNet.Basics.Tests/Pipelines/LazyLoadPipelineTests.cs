using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Pipelines
{
    public class LazyLoadPipelineTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        [Fact]
        public async Task LazyLoad_RunInSequence_StepsAreRunInSequence()
        {
            var pipeline = new LazyLoadPipeline(GetTransientServiceProvider<AddToListStep>());
            var items = new List<string>();
            //act
            await pipeline.RunAsync(items);

            //assert
            items.Count.Should().Be(pipeline.StepCount);
            for (var i = 0; i < pipeline.StepCount; i++)
                items[i].Should().Be(i.ToString());
        }
    }

    public class LazyLoadPipeline : Pipeline<List<string>>
    {
        public int StepCount { get; } = 11;

        public LazyLoadPipeline(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            for (var i = 0; i < StepCount; i++)
                AddStep<AddToListStep>(i.ToString());

        }
    }
}
