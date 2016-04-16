using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using NUnit.Framework;
using FluentAssertions;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class InMemDiagnosticsTests
    {
        [Test]
        public void Flush_LogIsCleared_ExistingLogEntriesAre()
        {
            var logger = new InMemDiagnostics();

            var logCount = 10;

            Parallel.For(0, logCount, (i) => logger.Log("blaaaa", LogLevel.Debug));

            logger.Count.Should().Be(logCount);

            logger.Clear();

            logger.Count.Should().Be(0);
        }

        [Test]
        public void ToString_PrinterFriendlyPrint_ContentIsWrittenToString()
        {
            var logger = new InMemDiagnostics();

            logger.Log("debug", LogLevel.Debug);
            logger.Log("info", LogLevel.Info);
            logger.Log("warning", LogLevel.Warning);
            logger.Log("error", LogLevel.Error);

            logger.ToString().Should().Be("Count:4");
        }

        [Test]
        public void HasFailed_Error_FailDetected()
        {
            var logger = new InMemDiagnostics();

            logger.Log("error", LogLevel.Error);

            logger.HasFailed().Should().BeTrue();
        }
        [Test]
        public void HasFailed_Critical_FailDetected()
        {
            var logger = new InMemDiagnostics();

            logger.Log("critical", LogLevel.Critical);

            logger.HasFailed().Should().BeTrue();
        }

        [Test]
        public void HasErrors_NoErrors_NoErrorsAreDetected()
        {
            var logger = new InMemDiagnostics();

            logger.Log("debug", LogLevel.Debug);
            logger.Log("info", LogLevel.Info);
            logger.Log("warning", LogLevel.Warning);
            logger.Metric("metric", 1.0);
            logger.Profile("profile", 1.Seconds());

            logger.HasFailed().Should().BeFalse();
        }

        [Test]
        public void Error_LogException_ExceptionIsFormattedNicely()
        {
            var logger = new InMemDiagnostics();

            var innerEx = new IOException("Im Inner Exception");
            var ex = new ApplicationException("Im outer exception", innerEx);
            logger.Log(null, LogLevel.Error, ex);
            var output = logger.Get<LogEntry>().Single(e => e.Level == LogLevel.Error).Message;

            output.Should().Be("System.ApplicationException: Im outer exception ---> System.IO.IOException: Im Inner Exception\r\n   --- End of inner exception stack trace ---");
        }

        [Test]
        public void Error_LogExceptionWithMessage_ExceptionIsFormattedNicely()
        {
            var logger = new InMemDiagnostics();

            var innerEx = new IOException("Im Inner Exception");
            var ex = new ApplicationException("Im outer exception", innerEx);
            logger.Log($"Hello World!", LogLevel.Error, ex);
            var output = logger.Get<LogEntry>().Single(e => e.Level == LogLevel.Error).Message;

            output.Should().Be("Hello World!\r\nSystem.ApplicationException: Im outer exception ---> System.IO.IOException: Im Inner Exception\r\n   --- End of inner exception stack trace ---");
        }

        [Test]
        public void Add_FilterTypes_TypesAreFiltered()
        {
            var logger = new InMemDiagnostics();

            logger.Log("debug", LogLevel.Debug);
            logger.Log("info", LogLevel.Info);
            logger.Log("warning", LogLevel.Warning);
            logger.Log("error", LogLevel.Error);
            logger.Metric("metric", 1.0);
            logger.Profile("profile", 1.Seconds());

            logger.Count.Should().Be(6);
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Info).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Warning).Count.Should().Be(1);
            logger.GetLogs(LogLevel.Error).Count.Should().Be(1);
        }
    }
}
