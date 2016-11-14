using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class ObservableTests
    {
        private readonly Observable<string> _primitiveObservable;
        
        public ObservableTests()
        {
            _primitiveObservable = new Observable<string>();
        }

        [Fact]
        public void Updated_TriggerEvent_EventIsFired()
        {
            //arrange
            _primitiveObservable.MonitorEvents();
            const string newValue = @"new Value";
            //act
            _primitiveObservable.Value = newValue;

            //assert
            _primitiveObservable.ShouldRaise(nameof(Observable<string>.Updating));
            _primitiveObservable.ShouldRaise(nameof(Observable<string>.Updated));
            _primitiveObservable.Value.Should().Be(newValue);
        }
    }
}
