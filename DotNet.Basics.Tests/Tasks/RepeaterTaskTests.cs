using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class RepeaterTaskTests
    {
        [Test]
        public void Task_WithFinallyNoExceptionInAction_ExceptionInfinallyIsFloated()
        {
            Action action = () => Repeat.Task(async () => await Task.Delay(10.Milliseconds()))
                .WithOptions(o =>
                {
                    o.MaxTries = 2;
                    o.Finally = () => { throw new System.IO.IOException("doh"); };
                })
                .UntilNoExceptions();

            action.ShouldThrow<System.IO.IOException>();
        }

        [Test]
        public void Task_WithFinallyExInBothFinallyAndAction_ExceptionsAreFloated()
        {
            Action action = () => Repeat.Task(() => { throw new ApplicationException(); })
           .WithOptions(o =>
           {
               o.MaxTries = 2;
               o.Finally = () => { throw new System.IO.IOException("doh"); };
           })
           .UntilNoExceptions();

            action.ShouldThrow<AggregateException>();
        }

        [Test]
        public void Task_WithFinally_FinallyIsExecutedEvenThoughTaskNeverCompleted()
        {
            var @finally = false;
            var actionExceptionCaught = false;

            try
            {
                var result = Repeat.Task(() => { throw new ApplicationException(); })
                    .WithOptions(o =>
                    {
                        o.MaxTries = 2;
                        o.Finally = () => { @finally = true; };
                    })
               .UntilNoExceptions();

                result.Should().BeFalse();
            }
            catch (ApplicationException)
            {
                actionExceptionCaught = true;
            }
            actionExceptionCaught.Should().BeTrue();
            @finally.Should().BeTrue("Finally should run");
        }


        [Test]
        public async Task Task_IgnoreExceptionsEvenIfUntilIsNotReached_ActionIsInvokedFiveTimesAndNoExceptions()
        {
            var doCounter = 0;
            const int until = 5;
            var result = await Repeat.Task(() => { doCounter++; throw new System.IO.IOException("buuh"); })
                .WithOptions(o =>
                    {
                        o.RetryDelay = 10.Milliseconds();
                        o.MaxTries = until;
                        o.IgnoreExceptionType = typeof(System.IO.IOException);
                    })
                .UntilAsync(() => false).ConfigureAwait(false);

            result.Should().BeFalse();
            doCounter.Should().Be(until);
        }


        [Test]
        public void Task_ExceptionIgnoredDuringRepeatUntilPredicateSetButNeverSucceeded_ActionIsInvokedFiveTimesAndExceptionIsThrownAtTheEnd()
        {
            var doCounter = 0;
            const int until = 5;
            Action runTask = () => Repeat.Task(() =>
            {
                doCounter++;
                throw new System.IO.IOException("buuh");
            })
            .WithOptions(o =>
            {
                o.RetryDelay = 10.Milliseconds();
                o.MaxTries = until;
            })
            .Until(() => false);


            runTask.ShouldThrow<System.IO.IOException>();
            doCounter.Should().Be(until);
            //we can assert result as it never returned properly
        }

        [Test]
        public async Task TaskAsync_InvokeFiveTimes_ActionIsInvokedFiveTimes()
        {
            var invoked = 0;
            var pinged = 0;
            const int until = 5;

            var result = await Repeat.Task(() => { invoked++; })
                .WithOptions(o =>
                {
                    o.RetryDelay = 10.Milliseconds();
                    o.Ping = () => pinged++;
                })
                .UntilAsync(() => invoked == until).ConfigureAwait(false);

            invoked.Should().Be(until);
            pinged.Should().Be(until);
            result.Should().BeTrue();
        }

        [Test]
        public void Task_UntilNoExceptionsAreThrown_LoopIsRunUntilNoExceptions()
        {
            const int stopThrowingExceptionsAt = 5;
            var tried = 0;

            var throwExceptionUntilXTriesDummeTask = new ThrowExceptionUntilXTriesDummeTask<System.IO.IOException>(stopThrowingExceptionsAt);

            var result = Repeat.Task(() => throwExceptionUntilXTriesDummeTask.DoSomething())
                .WithOptions(o =>
                {
                    o.Ping = () => tried++;
                })
                .UntilNoExceptions();

            tried.Should().Be(stopThrowingExceptionsAt + 1);
            result.Should().BeTrue();
        }

        [Test]
        public void Task_TimeOut_ActionTimesOut()
        {
            var doCounter = 0;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.Timeout = 1.Seconds();
                })
                .Until(() => false);

            //means that the action has been run a couple of times - we don't know the exact number since it depends on cpu time slots
            doCounter.Should().BeGreaterThan(2);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }

        [Test]
        public void Task_TimeOutWithNoUntil_ActionTimesOut()
        {
            var doCounter = 0;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.RetryDelay = 100.Milliseconds();
                    o.Timeout = 1.Seconds();
                })
                .Until(() => false);

            //means that the action has been run a couple of times - we don't know the exact number since it depends on cpu time slots
            doCounter.Should().BeGreaterThan(8);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }


        [Test]
        public void Task_MaxTries_ActionMaxedOutOnRetries()
        {
            var doCounter = 0;
            const int maxTries = 5;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action reached max tries and quit
            doCounter.Should().Be(maxTries);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }

        [Test]
        public void Task_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.Ping = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cyckes
            doCounter.Should().Be(maxTries);
            pingCounter.Should().Be(maxTries);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }

        [Test]
        public void TaskOnce_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.TaskOnce(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.Ping = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cycles
            doCounter.Should().Be(1);
            pingCounter.Should().Be(maxTries);
            //we never got a true result (the thing we were waiting for never succeeded)
            result.Should().BeFalse();
        }
        [Test]
        public void TaskOnceAsync_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.TaskOnce(ct => { doCounter++; return Task.CompletedTask; })
                .WithOptions(o =>
                {
                    o.Ping = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cycles
            doCounter.Should().Be(1);
            pingCounter.Should().Be(maxTries);
            //we never got a true result (the thing we were waiting for never succeeded)
            result.Should().BeFalse();
        }
    }
}

