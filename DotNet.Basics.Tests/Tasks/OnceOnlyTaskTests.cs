using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class OnceOnlyTaskTests
    {
        [Test]
        public void AsyncTask_Run_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlyAsyncTask((ct) => { counter++; return Task.CompletedTask; });
            Action action = async () => await onceOnlyAction.RunAsync(CancellationToken.None).ConfigureAwait(false);

            //invoke multiple times
            Run(action, 5);

            counter.Should().Be(1);
        }

        [Test]
        public void AsyncTask_RunWithException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlyAsyncTask((ct) => { counter++; throw new ArgumentException("buuh"); });
            Action action = async () => await onceOnlyAction.RunAsync(CancellationToken.None).ConfigureAwait(false);

            //invoke multiple times
            Run(action, 5);

            counter.Should().Be(1);
        }

        [Test]
        public void SyncTask_Run_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlySyncTask(() => counter++);
            Action action = onceOnlyAction.Run;

            //invoke multiple times
            Run(action, 5);

            counter.Should().Be(1);
        }

        [Test]
        public void SyncTask_RunWithException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlySyncTask(() => { counter++; throw new ArgumentException("buuh"); });
            Action action = onceOnlyAction.Run;

            //invoke multiple times
            Run(action, 5);

            counter.Should().Be(1);
        }

        private void Run(Action action, uint times)
        {
            for (var i = 0; i < times; i++)
            {
                try
                {
                    action.Invoke();
                }
                catch (ArgumentException)
                {
                    //ignore
                }
            }
        }
    }
}
