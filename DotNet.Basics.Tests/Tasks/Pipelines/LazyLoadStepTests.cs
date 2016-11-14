using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Core;
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
            var builder = new IocBuilder();
            var pipeline = new Pipeline(() => builder.Container);

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
            var builder = new IocBuilder();
            var pipeline = new Pipeline(() => builder.Container);

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
            exceptionEncountered.Should().BeOfType<DependencyResolutionException>();
        }
    }
}
