using System;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Pipelines
{
    public class LazyLoadStepTests(ITestOutputHelper output): TestWithHelpers(output)
    {
        [Fact]
        public void AssertLazyLoadSteps_MissingDirectRegistration_AssertHasEntries()
        {


            var errorMessage = string.Empty;
            var pipeline = new Pipeline<EventArgs>(GetTransientServiceProvider<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>());
            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            var assert = pipeline.AssertLazyLoadSteps(ref errorMessage);

            assert.Should().BeFalse();
            errorMessage.Should().Be("Failed to load GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingCtorRegistration_AssertHasEntries()
        {

            var errorMessage = string.Empty;
            var pipeline = new Pipeline<EventArgs>(GetTransientServiceProvider(typeof(GenericThatTakesAnotherConcreteClassAsArgStep<>)));


            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            //act
            var assert = pipeline.AssertLazyLoadSteps(ref errorMessage);

            assert.Should().BeFalse();
            errorMessage.Should().Be("Failed to load GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
        }
        
        [Fact]
        public async Task AddStep_StepIsRegisteredInContainer_StepIsResolved()
        {
            var pipeline = new Pipeline<EventArgs>(GetTransientServiceProvider<SimpleStep>());

            pipeline.AddStep<SimpleStep>();

            Exception exceptionEncountered;
            try
            {
                await pipeline.RunAsync(null);
                exceptionEncountered = null;
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
            }
            exceptionEncountered.Should().BeNull();
        }

        [Fact]
        public async Task AddStep_StepIsNotRegisteredProperlyInContainer_ExceptionIsThrownOnRun()
        {
            var pipeline = new Pipeline<EventArgs>(GetTransientServiceProvider<Task>());

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            Func<Task> act = async () => await pipeline.RunAsync(null);

            await act.Should().ThrowAsync<TaskNotResolvedFromServiceProviderException>();
        }
    }
}
