using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class ProfileEntryTests
    {
        [Test]
        public void Message_Formatting_ValueIsIncludedInMessage()
        {
            var duration = TimeSpan.FromSeconds(1000.5);

            var entry = new ProfileEntry(DateTime.MinValue,"MyTask", duration,DurationFormattingUnit.MilliSeconds);
            entry.ToString().Should().Be("0001-01-01 12:00:00 'MyTask' finished in 1000500.00 milliseconds");
            entry = new ProfileEntry(DateTime.MinValue, "MyTask", duration);
            entry.ToString().Should().Be("0001-01-01 12:00:00 'MyTask' finished in 1000.50 seconds");
            entry = new ProfileEntry(DateTime.MinValue, "MyTask", duration, DurationFormattingUnit.Minutes);
            entry.ToString().Should().Be("0001-01-01 12:00:00 'MyTask' finished in 16.68 minutes");
            entry = new ProfileEntry(DateTime.MinValue, "MyTask", duration, DurationFormattingUnit.Hours);
            entry.ToString().Should().Be("0001-01-01 12:00:00 'MyTask' finished in 0.28 hours");
        }


    }
}
