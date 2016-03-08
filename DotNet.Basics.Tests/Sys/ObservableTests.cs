using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class ObservableTests
    {
        private Observable<string> _primitiveObservable;

        [SetUp]
        public void Setup()
        {
            _primitiveObservable = new Observable<string>();
        }

        [Test]
        public void Updated_TriggerEvent_EventIsFired()
        {
            //arrange
            _primitiveObservable.MonitorEvents();
            const string newValue = @"new Value";
            //act
            _primitiveObservable.Value = newValue;

            //assert
            _primitiveObservable.ShouldRaise("Updating");
            _primitiveObservable.ShouldRaise("Updated");
            _primitiveObservable.Value.Should().Be(newValue);
        }
    }
}
