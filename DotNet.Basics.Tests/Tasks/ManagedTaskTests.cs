using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class ManagedTaskTests
    {
        [Fact]
        public async Task RunAsync_InlineTas_NoErrorsWereLogged()
        {
            var entriesLogged = new List<LogEntry>();

            var task = new ManagedTask<EventArgs>((args, log, ct) => { });
            task.EntryLogged += e => entriesLogged.Add(e);

            //act
            await task.RunAsync().ConfigureAwait(false);

            entriesLogged.Any(e => e.Level > LogLevel.Debug).Should().BeFalse();
        }

        [Fact]
        public async Task RunAsync_InlineTask_EntryIsLogged()
        {
            var message = "MyMessage.Lorem.Ipsum";
            var entriesLogged = new List<LogEntry>();

            var task = new ManagedTask<EventArgs>((args, log, ct) =>
            {
                log.LogError(message);
            });
            task.EntryLogged += e => entriesLogged.Add(e);

            //act
            await task.RunAsync().ConfigureAwait(false);

            entriesLogged.Count(e => e.Message == $"ManagedTask<EventArgs>: {message}").Should().Be(1);
        }

        [Fact]
        public async Task RunAsyncResult_NoCustomEntries_ResultIsGood()
        {
            var loggedEntries = 0;
            var task = new ManagedTask<EventArgs>((args, log, ct) => { });
            task.EntryLogged += entry => loggedEntries++;
            var result = await task.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Should().BeOfType<EventArgs>();
            loggedEntries.Should().Be(0);
        }

        [Fact]
        public async Task RunAsyncResult_Log_EntryIsLogged()
        {
            var value = 101;
            var inputArgs = new EventArgs<int>
            {
                Value = value
            };

            var task = new ManagedTask<EventArgs<int>>((args, log, ct) =>
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

            var task = new ManagedTask<EventArgs>((args, log, ct) => argsIsNotNull = args != null);
            await task.RunAsync(CancellationToken.None);

            argsIsNotNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            var argsValue = 12312313;

            var ctSource = new CancellationTokenSource();
            ctSource.Cancel();

            var task = new ManagedTask<EventArgs<int>>((args, log, ct) => { args.Value = argsValue; });

            using (var monitoredTask = task.Monitor())
            {
                //act
                await task.RunAsync(ctSource.Token).ConfigureAwait(false);

                monitoredTask.Should().Raise(nameof(task.Started));
                monitoredTask.Should().Raise(nameof(task.Ended));
            }
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args, log, ct) => { throw new ArgumentException(exMessage); });

            Exception capturedException = null;

            task.Ended += (name, e) => { capturedException = e; };

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
