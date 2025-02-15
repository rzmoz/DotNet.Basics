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
            await task.RunAsync(EventArgs.Empty);
        }

        [Fact]
        public async Task RunAsync_ArgsIsNull_ArgsIsNull()
        {
            string argsInput = null;
            var argsIsNull = false;

            var task = new ManagedTask<string>((args, ct) => argsIsNull = args == null);
            await task.RunAsync(argsInput, CancellationToken.None);

            argsIsNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            var argsValue = 12312313;

            var ctSource = new CancellationTokenSource();
            await ctSource.CancelAsync();

            var task = new ManagedTask<EventArgs<int>>((args, ct) => { args.Value = argsValue; });

            using var monitoredTask = task.Monitor();
            //act
            await task.RunAsync(null, ctSource.Token);

            monitoredTask.Should().Raise(nameof(task.Started));
            monitoredTask.Should().Raise(nameof(task.Ended));
        }

        [Fact]
        public async Task RunAsync_NonGenericBase_TaskIsRun()
        {
            ///VERY important that type is set to abstract base type
            var result = string.Empty;

            result.Should().BeEmpty();

            ManagedTask task = new ManagedTask<EventArgs<int>>((args, ct) => { result = "Hello World!"; });

            //act
            await task.RunAsync(null);

            //assert
            result.Should().Be("Hello World!");
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args, ct) => { throw new ArgumentException(exMessage); });

            Exception capturedException = null;

            task.Ended += (name, e) => { capturedException = e; };

            try
            {
                await task.RunAsync(null);
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
            await task.RunAsync(null);
            taskRan.Should().BeTrue();
        }
        [Fact]
        public async Task RunAsync_AsyncTask_TaskIsOnlyRunWhenInvoked()
        {
            var taskRan = false;

            var task = new ManagedTask<EventArgs>(async () =>
                {
                    await VoidTaskAsync();//ensure async execution
                    taskRan = true;
                }
            );

            taskRan.Should().BeFalse();
            await task.RunAsync(null);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.FromResult("");
        }
    }
}
