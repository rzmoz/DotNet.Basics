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
            var services = new ServiceCollection();
            services.AddTransient<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            var pipeline = new Pipeline();

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(services.BuildServiceProvider());

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Log.Count.Should().Be(1);
            assert.Log.Single().Message.Should().Be("Failed to load: GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
            assert.Log.Single().Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingCtorRegistration_AssertHasEntries()
        {
            var services = new ServiceCollection();
            services.AddTransient(typeof(GenericThatTakesAnotherConcreteClassAsArgStep<>));

            var pipeline = new Pipeline();

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(services.BuildServiceProvider());

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Log.Count.Should().Be(1);
            assert.Log.Single().Message.Should().Be("Failed to load: GenericThatTakesAnotherConcreteClassAsArgStep`1 - Unable to resolve service for type 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam' while attempting to activate 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]'.");
        }

        [Fact]
        public void GetTask_TaskExist_TaskIsRetrieved()
        {
            var lazyStep = new LazyLoadStep<EventArgs, ManagedTask<EventArgs>>("mystep", () => new ManagedTask<EventArgs>(""));

            var task = lazyStep.GetTask();

            task.Should().BeOfType(typeof(ManagedTask<>));
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
            var services = new ServiceCollection();
            var pipeline = new Pipeline();
            services.AddTransient<AddLogEntryStep>();

            pipeline.AddStep<AddLogEntryStep>(services.BuildServiceProvider());

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
            var services = new ServiceCollection();
            var pipeline = new Pipeline();

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(services.BuildServiceProvider());

            Func<Task> act = async () => await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<LazyLoadTaskFailedToLoadException>();
        }
    }
}
