using System;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class OnceOnlyActionTests
    {
        [Test]
        public void Invoke_Invoke_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlyAction(() => counter++);
            Action action = onceOnlyAction.Invoke;

            //invoke multiple times
            action.Invoke();
            action.Invoke();
            action.Invoke();
            action.Invoke();
            action.Invoke();

            counter.Should().Be(1);
        }

        [Test]
        public void Invoke_InvokeWhenActionThrowsException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            var onceOnlyAction = new OnceOnlyAction(() => { counter++; throw new Exception("buuh"); });
            Action action = onceOnlyAction.Invoke;

            //invoke multiple times
            action.Invoke();
            action.Invoke();
            action.Invoke();
            action.Invoke();
            action.Invoke();

            counter.Should().Be(1);
        }
    }
}
