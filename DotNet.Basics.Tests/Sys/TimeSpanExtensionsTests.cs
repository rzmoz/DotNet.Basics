using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class TimeSpanExtensionsTests
    {
        [Fact]
        public void ToTimeSpan_MilliSeconds_IsParsed()
        {
            var result = "100ms".ToTimeSpan();

            result.Should().Be(TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void ToTimeSpan_Seconds_IsParsed()
        {
            var result = "100s".ToTimeSpan();

            result.Should().Be(TimeSpan.FromSeconds(100));
        }

        [Fact]
        public void ToTimeSpan_Minutes_IsParsed()
        {
            var result = "100m".ToTimeSpan();

            result.Should().Be(TimeSpan.FromMinutes(100));
        }

        [Fact]
        public void ToTimeSpan_Hours_IsParsed()
        {
            var result = "100h".ToTimeSpan();

            result.Should().Be(TimeSpan.FromHours(100));
        }

        [Fact]
        public void ToTimeSpan_Days_IsParsed()
        {
            var result = "100d".ToTimeSpan();

            result.Should().Be(TimeSpan.FromDays(100));
        }

        [Fact]
        public void ToTimeSpan_Null_IsParsed()
        {
            Action act = () => ((string)null).ToTimeSpan();

            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ToTimeSpan_Empty_IsParsed()
        {
            Action act = () => string.Empty.ToTimeSpan();

            act.ShouldThrow<ArgumentException>().WithMessage("input is empty");
        }

        [Theory]
        [InlineData("100")]//missing unit
        [InlineData("100k")]//unknown unit
        [InlineData("1r00s")]//unknown number
        [InlineData("s")]//missing number
        public void ToTimeSpan_WrongFormat_FormatException(string input)
        {
            Action act = () => input.ToTimeSpan();

            act.ShouldThrow<FormatException>().WithMessage("Input must be in format {time}{unit} where time is an integer and unit is ms|s|m|h|d|t. Was: " + input);
        }
    }
}
