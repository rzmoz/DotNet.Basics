using System;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Diagnostics
{
    public class ProfileTests
    {
        [Fact]
        public void ToString_NotStarted_ToStringContainsStarted()
        {
            var stats = new Profiler("myStats");

            var toString = stats.ToString();

            toString.Should().Contain("'myStats' not started");
        }

        [Fact]
        public void ToString_Started_ToStringContainsFinished()
        {
            var time = new DateTime(2015, 10, 1, 12, 1, 1);
            var stats = new Profiler("myStats");

            stats.Start(time);

            stats.ToString().Should().Be("'myStats' started");
        }

        [Fact]
        public void ToString_Finished_ToStringContainsFinished()
        {
            var time = DateTime.UtcNow;
            var stats = new Profiler("myStats", time, time.AddDays(1));

            var toString = stats.ToString();

            toString.Should().Be("'myStats' finished in 1.00:00:00");
        }


        [Fact]
        public void Duration_TimeEvent_DurationIsCalculated()
        {
            var difference = new TimeSpan(0, 1, 1, 1, 100);
            var start = DateTime.MinValue;
            var end = start.Add(difference);

            var profile = new Profiler();

            //trace is not started so stop fails and duration is 0
            profile.Stop(end).Should().BeFalse();
            profile.Duration.Should().Be(0.Days());

            //we then succesfully start the trace but can only start it succesfully once
            profile.Start(start).Should().BeTrue();
            profile.Start(start).Should().BeFalse();
            profile.Duration.Should().Be(0.Days());

            //we stop the trace but only once succesfully
            profile.Stop(end).Should().BeTrue();
            profile.Stop(end).Should().BeFalse();

            //we verify values
            profile.StartTime.Should().Be(start);
            profile.EndTime.Should().Be(end);
            profile.Duration.Should().Be(difference);
        }
    }
}
