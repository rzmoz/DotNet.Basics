using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Repeating;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Repeating
{
    public class RepeaterTaskTests
    {
        [Fact]
        public void Ping_Success_PingOnImmediateSucccess_PingIsOnlyCalledOnRetry()
        {
            var pingWasCalled = false;
            var taskRan = false;
            Repeat.Task(() => { taskRan = true; })
                .WithOptions(o =>
                {
                    o.PingOnRetry = () => pingWasCalled = true;
                })
                .UntilNoExceptions();

            pingWasCalled.Should().BeFalse();
            taskRan.Should().BeTrue();
        }

        [Fact]
        public void Finally_NoExceptionInAction_ExceptionInfinallyIsFloated()
        {
            Action action = () => Repeat.Task(() => { })
                .WithOptions(o =>
                {
                    o.MaxTries = 2;
                    o.Finally = () => { throw new System.IO.IOException("doh"); };
                })
                .UntilNoExceptions();

            action.Should().Throw<System.IO.IOException>();
        }

        [Fact]
        public void Finally_ExInBothFinallyAndAction_BothExceptionsAreFloated()
        {
            Action action = () => Repeat.Task(() => { throw new ArithmeticException(); })
           .WithOptions(o =>
           {
               o.MaxTries = 2;
               o.Finally = () => { throw new System.IO.IOException("doh"); };
           })
           .UntilNoExceptions();

            action.Should().Throw<AggregateException>().WithInnerException<ArithmeticException>();
        }

        [Fact]
        public void Finally_EarlyBreak_FinallyIsExecutedEvenThoughTaskNeverCompleted()
        {
            var @finally = false;
            var actionExceptionCaught = false;

            try
            {
                var result = Repeat.Task(() => { throw new ArithmeticException(); })
                    .WithOptions(o =>
                    {
                        o.MaxTries = 2;
                        o.Finally = () => { @finally = true; };
                    })
               .UntilNoExceptions();

                result.Should().BeFalse();
            }
            catch (ArithmeticException)
            {
                actionExceptionCaught = true;
            }
            actionExceptionCaught.Should().BeTrue();
            @finally.Should().BeTrue("Finally should run");
        }

        [Fact]
        public async Task DontRethrowOnTaskFailedType_NamedExceptionsWillBeThrownOnTaskEnd_TaskFails()
        {
            var doCounter = 0;
            const int until = 5;
            var result = await Repeat.Task(() => { doCounter++; throw new System.IO.IOException("buuh"); })
                .WithOptions(o =>
                    {
                        o.RetryDelay = 10.Milliseconds();
                        o.MaxTries = until;
                        o.DontRethrowOnTaskFailedType = typeof(System.IO.IOException);
                        o.PingOnRetry = () => { Console.WriteLine(doCounter); };
                    })

                .UntilAsync(() => Task.FromResult(false)).ConfigureAwait(false);

            result.Should().BeFalse();
            doCounter.Should().Be(until);
        }

        [Fact]
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


            runTask.Should().Throw<System.IO.IOException>();
            doCounter.Should().Be(until);
            //we can assert result as it never returned properly
        }

        [Fact]
        public async Task TaskAsync_InvokeFiveTimes_ActionIsInvokedFiveTimes()
        {
            var invoked = 0;
            var pinged = 0;
            const int until = 5;

            var result = await Repeat.Task(() => { invoked++; })
                .WithOptions(o =>
                {
                    o.RetryDelay = 10.Milliseconds();
                    o.PingOnRetry = () => pinged++;
                })
                .UntilAsync(() => Task.FromResult(invoked == until)).ConfigureAwait(false);

            invoked.Should().Be(until);
            pinged.Should().Be(until - 1);
            result.Should().BeTrue();
        }

        [Fact]
        public void Task_UntilNoExceptionsAreThrown_LoopIsRunUntilNoExceptions()
        {
            const int stopThrowingExceptionsAt = 5;
            var tried = 0;

            var throwExceptionUntilXTriesDummeTask = new ThrowExceptionUntilXTriesDummeTask<System.IO.IOException>(stopThrowingExceptionsAt);

            var result = Repeat.Task(() => throwExceptionUntilXTriesDummeTask.DoSomething())
                .WithOptions(o =>
                {
                    o.PingOnRetry = () => tried++;
                })
                .UntilNoExceptions();

            tried.Should().Be(stopThrowingExceptionsAt);
            result.Should().BeTrue();
        }

        [Fact]
        public void Task_TimeOut_ActionTimesOut()
        {
            var doCounter = 0;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.RetryDelay = 0.Seconds();
                    o.Timeout = 1.Seconds();
                })
                .Until(() => false);

            //means that the action has been run a couple of times - we don't know the exact number since it depends on cpu time slots
            doCounter.Should().BeGreaterThan(2);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }

        [Fact]
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


        [Fact]
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

        [Fact]
        public void Task_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.Task(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.PingOnRetry = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cyckes
            doCounter.Should().Be(maxTries);
            pingCounter.Should().Be(maxTries - 1);
            //we never got a true result (the thing we we'e waiting for never succeeded)
            result.Should().BeFalse();
        }

        [Fact]
        public void TaskOnce_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.TaskOnce(() => { doCounter++; })
                .WithOptions(o =>
                {
                    o.PingOnRetry = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cycles
            doCounter.Should().Be(1);
            pingCounter.Should().Be(maxTries - 1);
            //we never got a true result (the thing we were waiting for never succeeded)
            result.Should().BeFalse();
        }
        [Fact]
        public void TaskOnceAsync_InvokeAndPingback_ActionsAreInvokedTheRightNumberOfTimes()
        {
            var doCounter = 0;
            var pingCounter = 0;
            const int maxTries = 5;
            var result = Repeat.TaskOnce(() => { doCounter++; return Task.FromResult(""); })
                .WithOptions(o =>
                {
                    o.PingOnRetry = () => { pingCounter++; };
                    o.RetryDelay = 10.Milliseconds();
                    o.MaxTries = maxTries;
                })
                .Until(() => false);

            //means that the action has only been run once even though we waited 5 cycles
            doCounter.Should().Be(1);
            pingCounter.Should().Be(maxTries - 1);
            //we never got a true result (the thing we were waiting for never succeeded)
            result.Should().BeFalse();
        }
    }
}