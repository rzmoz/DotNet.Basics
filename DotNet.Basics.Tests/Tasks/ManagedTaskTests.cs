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
        public async Task Ctor_NoTask_NoExceptionIsThrown()
        {
            var task = new ManagedTask<EventArgs>("MyTask");
            
            //act
            await task.RunAsync(EventArgs.Empty).ConfigureAwait(false);
        }

        [Fact]
        public async Task MessagedLogged_LogMessage_MessageHasContext()
        {
            var taskReceived = string.Empty;
            var message = "Hello World!";
            var taskName = "MyTask";
            var task = new ManagedTask<EventArgs>(taskName, (args, log, ct) => log.Debug(message));

            task.MessageLogged += (lvl, msg, e) => taskReceived = msg;

            //act
            await task.RunAsync(EventArgs.Empty).ConfigureAwait(false);

            //assert
            taskReceived.Should().Be($"{taskName} / {message}");
        }

        [Fact]
        public async Task RunAsync_ArgsIsNull_ArgsIsNull()
        {
            string argsInput = null;
            var argsIsNull = false;

            var task = new ManagedTask<string>((args, log, ct) => argsIsNull = args == null);
            await task.RunAsync(argsInput, CancellationToken.None);

            argsIsNull.Should().BeTrue();
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
                await task.RunAsync(null, ctSource.Token).ConfigureAwait(false);

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
                await task.RunAsync(null).ConfigureAwait(false);
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
            await task.RunAsync(null).ConfigureAwait(false);
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
            await task.RunAsync(null).ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.FromResult("");
        }
    }
}
