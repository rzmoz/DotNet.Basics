using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class LazyLoadStepTests
    {
        [Fact]
        public void AssertLazyLoadSteps_MissingDirectRegistration_AssertHasEntries()
        {
            var errorMessage = string.Empty;
            var pipeline = new Pipeline<EventArgs>(services => services.AddTransient<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>());
            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();
            pipeline.EntryLogged += le => errorMessage = le.Message;

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Should().BeFalse();
            errorMessage.Should().Be("Pipeline<EventArgs> / Failed to load: GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingCtorRegistration_AssertHasEntries()
        {
            var errorMessage = string.Empty;
            var pipeline = new Pipeline<EventArgs>(services => services.AddTransient(typeof(GenericThatTakesAnotherConcreteClassAsArgStep<>)));


            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();
            pipeline.EntryLogged += le => errorMessage = le.Message;

            //act
            var assert = pipeline.AssertLazyLoadSteps();

            assert.Should().BeFalse();
            errorMessage.Should().Be("Pipeline<EventArgs> / Failed to load: GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
        }

        [Fact]
        public void GetTask_Fails_ExceptionIsBubbled()
        {
            var lazyStep = new LazyLoadStep<EventArgs, ManagedTask<EventArgs>>("mystep", () => { throw new ArgumentException(); });

            Action action = () => lazyStep.GetTask();

            action.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public async Task AddStep_StepIsRegisteredInContainer_StepIsResolved()
        {
            var pipeline = new Pipeline<EventArgs>(services => services.AddTransient<AddLogEntryStep>());

            pipeline.AddStep<AddLogEntryStep>();

            Exception exceptionEncountered;
            try
            {
                await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);
                exceptionEncountered = null;
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
            }
            exceptionEncountered.Should().BeNull();
        }

        [Fact]
        public void AddStep_StepIsNotRegisteredProperlyInContainer_ExceptionIsThrownOnRun()
        {
            var pipeline = new Pipeline<EventArgs>();

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            Func<Task> act = async () => await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<TaskNotResolvedFromServiceProviderException>();
        }
    }
}
