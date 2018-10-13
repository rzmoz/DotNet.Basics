using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class ManagedTaskTests
    {
        [Fact]
        public async Task RunAsyncResult_NoCustomEntries_ResultIsGood()
        {
            var loggedEntries = 0;
            var task = new ManagedTask<EventArgs>((args, ct) => { });
            task.EntryLogged += (name, le) => loggedEntries++;
            var result = await task.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Should().BeOfType<EventArgs>();
            loggedEntries.Should().Be(2);//start+end are logged
        }

        [Fact]
        public async Task RunAsyncResult_Log_EntryIsLogged()
        {
            var value = 101;
            var inputArgs = new EventArgs<int>
            {
                Value = value
            };

            var task = new ManagedTask<EventArgs<int>>((args, t) =>
            {
                args.Value++;
            });
            var result = await task.RunAsync(inputArgs, CancellationToken.None).ConfigureAwait(false);

            inputArgs.Value.Should().Be(value + 1);
            result.Value.Should().Be(value + 1);
        }

        [Fact]
        public async Task RunAsync_ArgsIsNull_ArgsIsDefault()
        {
            var argsIsNotNull = false;

            var task = new ManagedTask<EventArgs>((args, ct) => argsIsNotNull = args != null);
            await task.RunAsync(CancellationToken.None);

            argsIsNotNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            var argsValue = 12312313;

            EventArgs<int> startedArgs = null;
            EventArgs<int> endedArgs = null;

            var ctSource = new CancellationTokenSource();
            ctSource.Cancel();

            var task = new ManagedTask<EventArgs<int>>((args, ct) => { args.Value = argsValue; });

            task.Started += (name, args) => { startedArgs = args; };
            task.Ended += (name, args, e) => { endedArgs = args; };

            //act
            await task.RunAsync(ctSource.Token).ConfigureAwait(false);

            //assert
            startedArgs.Should().NotBeNull();
            endedArgs.Should().NotBeNull();
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args, ct) => { throw new ArgumentException(exMessage); });

            Exception capturedException = null;

            task.Ended += (name, args, e) => { capturedException = e; };

            try
            {
                await task.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //ignore
            }

            //assert
            capturedException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async Task TaskStarted_EventRaising_EndedEventIsRaised()
        {
            var eventRaised = false;

            var task = new ManagedTask<EventArgs<int>>((args, ct) => { });

            task.Started += (name, args) =>
            {
                eventRaised = true;
            };

            await task.RunAsync(CancellationToken.None);

            eventRaised.Should().BeTrue("Event raised");
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
