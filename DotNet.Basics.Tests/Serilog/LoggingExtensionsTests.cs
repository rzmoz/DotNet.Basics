/*using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.Serilog;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace DotNet.Basics.Tests.Serilog
{
    public class LoggingExtensionsTests
    {
        [Fact]
        public void ToSeriLogEventLevel_Conversion_LevelIsConvertedFromMicrosoftExtensionLogLevel()
        {
            LoogLevel.None.ToSeriLogEventLevel().Should().Be(LogEventLevel.Verbose);
            LoogLevel.Trace.ToSeriLogEventLevel().Should().Be(LogEventLevel.Verbose);
            LoogLevel.Debug.ToSeriLogEventLevel().Should().Be(LogEventLevel.Debug);
            LoogLevel.Info.ToSeriLogEventLevel().Should().Be(LogEventLevel.Info);
            LoogLevel.Warning.ToSeriLogEventLevel().Should().Be(LogEventLevel.Warning);
            LoogLevel.Error.ToSeriLogEventLevel().Should().Be(LogEventLevel.Error);
            LoogLevel.Critical.ToSeriLogEventLevel().Should().Be(LogEventLevel.Fatal);
        }
    }
}*/
