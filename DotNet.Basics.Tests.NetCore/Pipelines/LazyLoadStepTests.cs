using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using DotNet.Basics.Autofac;
using DotNet.Basics.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class LazyLoadStepTests
    {
        private readonly AutofacBuilder _builder;

        public LazyLoadStepTests()
        {
            _builder = new AutofacBuilder(false);
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingDirectRegistration_AssertHasIssues()
        {
            var pipeline = new Pipeline(() => _builder.Container);

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Issues.Count.Should().Be(1);
            assert.Issues.Single().Message.Should().Be("Step not registered: GenericThatTakesAnotherConcreteClassAsArgStep`1");
            assert.Issues.Single().Exception.Should().BeOfType<ComponentNotRegisteredException>();
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingCtorRegistration_AssertHasIssues()
        {
            var pipeline = new Pipeline(() => _builder.Container);
            _builder.RegisterGeneric(typeof(GenericThatTakesAnotherConcreteClassAsArgStep<>));

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Issues.Count.Should().Be(1);
            assert.Issues.Single().Message.Should().Be("Step ctor failed in Step GenericThatTakesAnotherConcreteClassAsArgStep`1: None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[System.EventArgs]' can be invoked with the available services and parameters:\r\nCannot resolve parameter 'DotNet.Basics.Tests.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam argStepDependsOn' of constructor 'Void .ctor(DotNet.Basics.Tests.Pipelines.PipelineHelpers.ClassThatTakesAnAbstractClassAsCtorParam)'.");
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

            action.ShouldThrowExactly<ArgumentException>();
        }

        [Fact]
        public async Task AddStep_StepIsRegisteredInContainer_StepIsResovled()
        {
            var pipeline = new Pipeline(() => _builder.Container);
            _builder.RegisterPipelineSteps<AddIssueStep>();

            pipeline.AddStep<AddIssueStep>();

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
        public async Task AddStep_StepIsNotRegisteredProperlyInContainer_ExceptionIsThrownOnRun()
        {
            var pipeline = new Pipeline(() => _builder.Container);

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            Exception exceptionEncountered = null;
            try
            {
                await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
            }
            exceptionEncountered.Should().BeOfType<ComponentNotRegisteredException>();
        }
    }
}
