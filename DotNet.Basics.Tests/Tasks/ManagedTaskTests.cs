using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Collections;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class ManagedTaskTests
    {
        [Fact]
        public async Task RunAsyncResult_NoEntries_ResultIsGood()
        {
            var task = new ManagedTask<EventArgs>((args, log, ct) => { });
            var result = await task.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Log.None().Should().BeTrue();
        }

        [Fact]
        public async Task RunAsyncResult_EntryFound_ResultHasEntries()
        {
            var entry = "RunAsyncResult_EntryFound_ResultHasEntries";
            var inputValue = 234232424;
            var inputArgs = new EventArgs<int> { Value = inputValue };

            var task = new ManagedTask<EventArgs<int>>((args, log, ct) =>
            {
                log.Add(LogLevel.Error, entry);
                args.Value++;
            });
            var result = await task.RunAsync(inputArgs, CancellationToken.None).ConfigureAwait(false);

            result.Log.Any().Should().BeTrue();
            result.Log.Single().Message.Should().Be(entry);
        }

        [Fact]
        public async Task RunAsync_ArgsIsNull_ArgsIsDefault()
        {
            var argsIsNotNull = false;

            var task = new ManagedTask<EventArgs>((args, log, ct) => argsIsNotNull = args != null);
            await task.RunAsync(CancellationToken.None);

            argsIsNotNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            var argsValue = 12312313;

            TaskResult startedArgs = null;
            TaskResult endedArgs = null;

            var ctSource = new CancellationTokenSource();
            ctSource.Cancel();

            var task = new ManagedTask<EventArgs<int>>((args, log, ct) => { args.Value = argsValue; });

            task.Started += (args) => { startedArgs = args; };
            task.Ended += (args) => { endedArgs = args; };

            await task.RunAsync(ctSource.Token).ConfigureAwait(false);

            //assert - started
            startedArgs.Should().NotBeNull();
            startedArgs.Should().NotBeNull();
            startedArgs.Name.Should().Be(task.GetType().Name, "Expected Name");

            //endedArgs.Problems.SelectMany(p => p.Exception).Should().BeEmpty();

            endedArgs.Name.Should().Be(startedArgs.Name);
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args, log, ct) => { throw new ArgumentException(exMessage); });

            TaskResult endedArgs = null;

            task.Ended += (args) => { endedArgs = args; };

            try
            {
                await task.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //ignore
            }

            //assert
            endedArgs.Log.Select(i => i.Exception).Single(e => e != null).Should().BeOfType<ArgumentException>();
            endedArgs.Log.Select(i => i.Exception).Single(e => e != null).Message.Should().Be(exMessage);
        }

        [Fact]
        public async Task TaskStarted_EventRaising_EndedEventIsRaised()
        {
            var eventRaised = false;
            var observedName = "";

            var task = new ManagedTask<EventArgs<int>>((args, log, ct) => { });

            task.Started += (args) =>
            {
                eventRaised = true;
                observedName = args.Name;
            };

            await task.RunAsync(CancellationToken.None);

            eventRaised.Should().BeTrue("Event raised");
            observedName.Should().Be(task.GetType().Name, "Expected Name");
        }

        [Fact]
        public async Task RunAsync_SyncTask_TaskIsOnlyRunWhenInvoked()
        {
            var taskRan = false;

            var task = new ManagedTask<EventArgs>(() => taskRan = true);

            taskRan.Should().BeFalse();
            await task.RunAsync().ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }
        [Fact]
        public async Task RunAsync_AsyncTask_TaskIsOnlyRunWhenInvoked()
        {
            var taskRan = false;

            var task = new ManagedTask<EventArgs>(async () =>
                {
                    await VoidTaskAsync().ConfigureAwait(false);//ensure async execution
                    taskRan = true;
                }
            );

            taskRan.Should().BeFalse();
            await task.RunAsync().ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.FromResult("");
        }
    }
}
