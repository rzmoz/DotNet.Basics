using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class LazyLoadStepTests
    {
        private readonly IocBuilder _builder;

        public LazyLoadStepTests()
        {
            _builder = new IocBuilder(false);
        }

        [Fact]
        public void AssertLazyLoadSteps_MissingRegistrations_AssertFails()
        {
            var pipeline = new Pipeline(() => _builder.Container);

            pipeline.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>();

            var assert = pipeline.AssertLazyLoadSteps();

            assert.Issues.Count.Should().Be(1);
            assert.Issues.Single().Message.Should().StartWith("Lazy Step failed to load: GenericThatTakesAnotherConcreteClassAsArgStep`1 - The requested service 'DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers.GenericThatTakesAnotherConcreteClassAsArgStep`1[[System.EventArgs, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]' has not been registered. To avoid this exception, either register a component to provide the service, check for service registration using IsRegistered(), or use the ResolveOptional() method to resolve an optional dependency.");
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
