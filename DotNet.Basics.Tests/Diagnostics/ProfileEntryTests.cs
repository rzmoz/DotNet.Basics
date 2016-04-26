using System;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class ProfileEntryTests
    {
        [Test, Ignore]
        public void Message_Formatting_ValueIsIncludedInMessage()
        {
            var duration = TimeSpan.FromSeconds(1000.5);

            var entry = new ProfileEntry(DateTime.MinValue, "MyTask", duration);
            entry.ToString(DurationFormattingUnit.MilliSeconds).Should().Be("0001-01-01 12:00:00 'MyTask' finished in 1000500.00 milliseconds");
            entry.ToString(DurationFormattingUnit.Seconds).Should().Be("0001-01-01 12:00:00 'MyTask' finished in 1000.50 seconds");
            entry.ToString(DurationFormattingUnit.Minutes).Should().Be("0001-01-01 12:00:00 'MyTask' finished in 16.68 minutes");
            entry.ToString(DurationFormattingUnit.Hours).Should().Be("0001-01-01 12:00:00 'MyTask' finished in 0.28 hours");
        }
    }
}
