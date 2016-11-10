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
        public async Task RunAsync_ArgsIsNull_ArgsIsDefault()
        {
            var argsIsNotNull = false;

            var task = new ManagedTask<EventArgs>((args, ct) => argsIsNotNull = args != null);
            await task.RunAsync(null, CancellationToken.None);

            argsIsNotNull.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_EventRaising_StartAndEndedEventAreRaised()
        {
            var myKey = "!sfrwe";
            var myValue = Guid.NewGuid().ToString();
            var argsValue = 12312313;

            TaskStartedEventArgs startedArgs = null;
            TaskEndedEventArgs endedArgs = null;
            EventArgs<int> resultArgs = null;
            var ctSource = new CancellationTokenSource();
            ctSource.Cancel();

            var task = new ManagedTask<EventArgs<int>>((args, ct) => { args.Value = argsValue; });
            task.Properties[myKey] = myValue;

            task.Started += (args) => { startedArgs = args; };
            task.Ended += (args) => { endedArgs = args; };

            resultArgs = await task.RunAsync(null, ctSource.Token);

            //assert - started
            startedArgs.Should().NotBeNull();
            startedArgs.Should().NotBeNull();
            startedArgs.Name.Should().Be(task.GetType().Name, "Expected Name");
            startedArgs.TaskProperties[myKey].Should().Be(myValue);

            endedArgs.Exception.Should().BeNull();
            endedArgs.WasCancelled.Should().BeTrue();
            endedArgs.Name.Should().Be(startedArgs.Name);
            endedArgs.TaskProperties[myKey].Should().Be(startedArgs.TaskProperties[myKey]);

            resultArgs.Value.Should().Be(argsValue);
        }

        [Fact]
        public async Task RunAsync_Exception_ExceptionIsCapturedInTaskEndEvent()
        {
            var exMessage = "buuh";
            var task = new ManagedTask<EventArgs<int>>((args, ct) => { throw new ArgumentException(exMessage); });

            TaskEndedEventArgs endedArgs = null;

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
            endedArgs.Exception.Should().BeOfType<ArgumentException>();
            endedArgs.Exception.Message.Should().Be(exMessage);
        }

        [Fact]
        public async Task TaskStarted_EventRaising_EndedEventIsRaised()
        {
            var eventRaised = false;
            var myKey = "!sfrwe";
            var myValue = Guid.NewGuid().ToString();
            var observedName = "";
            var observedValue = "";

            var task = new ManagedTask<EventArgs<int>>((args, ct) => { });
            task.Properties[myKey] = myValue;

            task.Started += (args) =>
            {
                eventRaised = true;
                observedName = args.Name;
                observedValue = args.TaskProperties[myKey];
            };

            await task.RunAsync(null, CancellationToken.None);

            eventRaised.Should().BeTrue("Event raised");
            observedName.Should().Be(task.GetType().Name, "Expected Name");
            observedValue.Should().Be(myValue);
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
