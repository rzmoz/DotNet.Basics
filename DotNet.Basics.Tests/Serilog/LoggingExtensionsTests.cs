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
            LogLevel.None.ToSeriLogEventLevel().Should().Be(LogEventLevel.Verbose);
            LogLevel.Trace.ToSeriLogEventLevel().Should().Be(LogEventLevel.Verbose);
            LogLevel.Debug.ToSeriLogEventLevel().Should().Be(LogEventLevel.Debug);
            LogLevel.Information.ToSeriLogEventLevel().Should().Be(LogEventLevel.Information);
            LogLevel.Warning.ToSeriLogEventLevel().Should().Be(LogEventLevel.Warning);
            LogLevel.Error.ToSeriLogEventLevel().Should().Be(LogEventLevel.Error);
            LogLevel.Critical.ToSeriLogEventLevel().Should().Be(LogEventLevel.Fatal);
        }
    }
}*/
