using System;
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

            var task = new ManagedTask<string>((args) => argsIsNull = args == null);
            await task.RunAsync(argsInput);

            argsIsNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            // ReSharper disable once AsyncVoidLambda
            var task = new ManagedTask<EventArgs>(async _ => { await Task.Delay(1.Seconds()); });

            using var monitoredTask = task.Monitor();
            //act
            await task.RunAsync(EventArgs.Empty);

            monitoredTask.Should().Raise(nameof(task.Started));
            monitoredTask.Should().Raise(nameof(task.Ended));
        }

        [Fact]
        public async Task RunAsync_NonGenericBase_TaskIsRun()
        {
            //VERY important that type is set to abstract base type
            int exitCode = 123412; 
            ManagedTask task = new ManagedTask<EventArgs>(_ => Task.FromResult(exitCode));

            //act
            var observedExitCode = await task.RunAsync(EventArgs.Empty);

            //assert
            observedExitCode.Should().Be(exitCode);
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args) => throw new ArgumentException(exMessage));

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
