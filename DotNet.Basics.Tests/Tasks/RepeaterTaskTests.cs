using System;
using System.IO;
using System.Threading;
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
                    .WithMaxTries(2)
                    .WithFinally(() => { throw new IOException("doh"); })
                    .UntilNoExceptions()
                    .Sync();

            action.ShouldThrow<IOException>();
        }

        [Test]
        public void Task_WithFinallyExInBothFinallyAndAction_ExceptionsAreFloated()
        {
            Action action = () => Repeat.Task(() =>
           {
               throw new ApplicationException(); //run action that throw exception
           })
                      .WithMaxTries(2)
                      .WithFinally(() => { throw new IOException("doh"); })
                      .UntilNoExceptions()
                      .Sync();
            action.ShouldThrow<AggregateException>();

        }

        [Test]
        public void Task_WithFinally_FinallyIsExecutedEvenThoughTaskNeverCompleted()
        {
            var @finally = false;
            var actionExceptionCaught = false;

            try
            {
                var result = Repeat.Task(() =>
               {
                   throw new ApplicationException();//run action that throw exception
               })
                                                  .WithMaxTries(2)
                                                  .WithFinally(() => { @finally = true; })
                                                  .UntilNoExceptions()
                                                  .Sync();
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
            var result = await Repeat.Task(() => { doCounter++; throw new IOException("buuh"); })
                .WithNoRetryDelay()
                .WithMaxTries(until)
                .Until(() => false)
                .IgnoreExceptionsOfType(typeof(IOException))
                .Async().ConfigureAwait(false);

            result.Should().BeFalse();
            doCounter.Should().Be(until);
        }


        [Test]
        public void Task_ExceptionIgnoredDuringRepeatUntilPredicateSetButNeverSucceeded_ActionIsInvokedFiveTimesAndExceptionIsThrownAtTheEnd()
        {
            var doCounter = 0;
            const int until = 5;
            Action runTask = () => Repeat.Task(() => { doCounter++; throw new IOException("buuh"); })
                .WithNoRetryDelay()
                .WithMaxTries(until)
                .Until(() => false)
                .Sync();


            runTask.ShouldThrow<IOException>();
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
                .WithNoRetryDelay()
                .WithPing(() => pinged++)
                 .Until(() => invoked == until)
                 .Async().ConfigureAwait(false);

            invoked.Should().Be(until);
            pinged.Should().Be(until);
            result.Should().BeTrue();
        }

        [Test]
        public void Task_UntilNoExceptionsAreThrown_LoopIsRunUntilNoExceptions()
        {
            const int stopThrowingExceptionsAt = 5;
            var tried = 0;

            var throwExceptionUntilXTriesDummeTask = new ThrowExceptionUntilXTriesDummeTask<IOException>(stopThrowingExceptionsAt);

            var result = Repeat.Task(() => throwExceptionUntilXTriesDummeTask.DoSomething())
                 .WithPing(() => tried++)
                 .UntilNoExceptions()
                 .Sync();

            tried.Should().Be(stopThrowingExceptionsAt + 1);
            result.Should().BeTrue();
        }

        [Test]
        public void Task_NoUntilSpecified_ExceptionIsThrownSinceLoopIsLikelyToRunForever()
        {
            Action action = () => Repeat.Task(() => { }).Sync();
            action.ShouldThrow<NoStopConditionIsSetException>();
        }

        [Test]
        public void Task_TimeOut_ActionTimesOut()
        {
            var doCounter = 0;
            var result = Repeat.Task(() => { doCounter++; })
                .Until(() => false)
                .WithTimeout(1.Seconds())
                .Sync();

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
                .WithRetryDelay(100.Milliseconds())
                .WithTimeout(1.Seconds())
                .Until(() => false)
                .Sync();

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
                .WithNoRetryDelay()
                .WithMaxTries(maxTries)
                .Until(() => false)
                .Sync();

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
                .WithPing(() => { pingCounter++; })
                .WithMaxTries(maxTries)
                .WithNoRetryDelay()
                .Until(() => false)
                .Sync();

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
                .WithPing(() => { pingCounter++; })
                .WithMaxTries(maxTries)
                .WithNoRetryDelay()
                .Until(() => false)
                .Sync();

            //means that the action has only been run once even though we waited 5 cyckes
            doCounter.Should().Be(1);
            pingCounter.Should().Be(maxTries);
            //we never got a true result (the thing we were waiting for never succeeded)
            result.Should().BeFalse();
        }
    }
}

