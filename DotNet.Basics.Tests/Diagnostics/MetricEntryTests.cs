using System;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class MetricEntryTests
    {
        [Test, Ignore]
        public void Message_Formatting_ValueIsIncludedInMessage()
        {
            var name = "myMetric";
            var value = 15.0;

            var entry = new MetricEntry(DateTime.MinValue, name, value);
            entry.Name.Should().Be(name);
            entry.Value.Should().Be(value);
            entry.ToString().Should().Be($"0001-01-01 12:00:00 '{name}':{value}");
        }
    }
}
