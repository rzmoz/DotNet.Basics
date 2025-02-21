using System;
using System.Collections.Generic;
using DotNet.Basics.Serilog.Looging;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Serilog.Diagnostics
{
    public class NullLoogTests
    {
        [Fact]
        public void Events_Null_NoEventsAreRaised()
        {
            var log = NullLoog.Instance;

            var messages = new List<string>();

            log.MessageLogged += (lvl, msg, e) => messages.Add($"{lvl}{msg}{e}");
            log.TimingLogged += (lvl, name, ev, d) => messages.Add($"{lvl}{name}{ev}{d}");

            log.Info("hello");
            log.Timing(LoogLevel.Debug, "name", "event", TimeSpan.FromDays(1));

            messages.Count.Should().Be(0);
        }
    }
}
