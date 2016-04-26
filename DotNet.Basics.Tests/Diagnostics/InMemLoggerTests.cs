using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class InMemLoggerTests
    {
        [Test]
        public void Flush_LogIsCleared_ExistingLogEntriesAre()
        {
            var logger = new InMemLogger();

            var logCount = 10;

            Parallel.For(0, logCount, (i) => logger.LogDebug("blaaaa"));

            logger.Count.Should().Be(logCount);
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(logCount);

            logger.Clear();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(0);
            logger.Count.Should().Be(0);
        }

        [Test]
        public void ToString_PrinterFriendlyPrint_ContentIsWrittenToString()
        {
            var logger = new InMemLogger();

            logger.LogDebug("debug");
            logger.LogVerbose("verbose");
            logger.LogInformation("info");
            logger.LogWarning("warning");
            logger.LogError("error");
            logger.LogCritical("critical");

            logger.ToString().Should().Be("Count:6;Failed:True");
        }

        [Test]
        [TestCase(LogLevel.Critical)]
        [TestCase(LogLevel.Error)]
        public void HasFailed_LoggerContainsFailures_FailDetected(LogLevel logLevel)
        {
            var logger = new InMemLogger();

            logger.Log("failure", logLevel);

            logger.HasFailed().Should().BeTrue(logLevel.ToName());
        }
        [Test]
        public void HasFailed_LoggerNotFailed_FailNotDetected(LogLevel logLevel)
        {
            var logger = new InMemLogger();

            logger.LogDebug("not failure");
            logger.LogVerbose("not failure");
            logger.LogInformation("not failure");
            logger.LogWarning("not failure");

            logger.HasFailed().Should().BeFalse(logLevel.ToName());
        }

        [Test]
        public void Error_LogException_ExceptionIsFormattedNicely()
        {
            var logger = new InMemLogger();

            var innerEx = new IOException("Im Inner Exception");
            var ex = new ApplicationException("Im outer exception", innerEx);
            var timeStamp = new DateTime(2000, 01, 01, 01, 01, 01);
            logger.Log(timeStamp, null, LogLevel.Error, ex);

            var entry = logger.Get<LogEntry>().Single(e => e.Level == LogLevel.Error);

            var output = entry.Message;

            output.Should().Be("[2000-01-01 01:01:01] <Error> \r\nSystem.ApplicationException: Im outer exception ---> System.IO.IOException: Im Inner Exception\r\n   --- End of inner exception stack trace ---");
        }

        [Test]
        public void Error_LogExceptionWithMessage_ExceptionIsFormattedNicely()
        {
            var logger = new InMemLogger();

            var innerEx = new IOException("Im Inner Exception");
            var ex = new ApplicationException("Im outer exception", innerEx);
            var timeStamp = new DateTime(2000, 01, 01, 01, 01, 01);
            var message = "Hello World!";
            logger.Log(timeStamp, message, LogLevel.Error, ex);
            var output = logger.Get<LogEntry>().Single(e => e.Level == LogLevel.Error).Message;

            output.Should().Be($"[2000-01-01 01:01:01] <Error> {message}\r\nSystem.ApplicationException: Im outer exception ---> System.IO.IOException: Im Inner Exception\r\n   --- End of inner exception stack trace ---");
        }

        [Test]
        public void Add_FilterTypes_TypesAreFiltered()
        {
            var logger = new InMemLogger();

            logger.LogDebug("debug");
            logger.LogVerbose("verbose");
            logger.LogInformation("info");
            logger.LogWarning("warning");
            logger.LogError("error");
            logger.LogCritical("critical");

            logger.Count.Should().Be(6);
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Verbose).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Information).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Warning).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Error).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Critical).Count.Should().Be(1);
        }
    }
}
