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
        public void Log_AddEntryCollection_EntriesAreAdded()
        {
            var logger = new InMemLogger();
            logger.Count.Should().Be(0);

            var logentry = new LogEntry(DateTime.UtcNow, "", LogLevel.Critical);

            logger.Log(logentry, logentry, logentry, logentry, logentry, logentry);

            logger.Count.Should().Be(6);
        }


        [Test]
        public void Flush_LogIsCleared_ExistingLogEntriesAre()
        {
            var logger = new InMemLogger();

            var logCount = 10;

            Parallel.For(0, logCount, (i) => logger.LogDebug("blaaaa"));

            logger.Count.Should().Be(logCount);
            logger.Get(LogLevel.Debug).Count.Should().Be(logCount);

            logger.Clear();
            logger.Get(LogLevel.Debug).Count.Should().Be(0);
            logger.Count.Should().Be(0);
        }

        [Test]
        public void ToString_PrinterFriendlyPrint_ContentIsWrittenToString()
        {
            var logger = new InMemLogger();

            logger.LogDebug("debug");
            logger.LogTrace("trace");
            logger.LogInformation("info");
            logger.LogWarning("warning");
            logger.LogError("error");
            logger.LogCritical("critical");

            //act
            var toString = logger.ToString();
            toString.Should().Be("Count:6;Debugs:1;Verboses:1;Infos:1;Warnings:1;Errors:1;Criticals:1");
        }

        [Test]
        [TestCase(LogLevel.Debug, false)]
        [TestCase(LogLevel.Trace, false)]
        [TestCase(LogLevel.Information, false)]
        [TestCase(LogLevel.Warning, false)]
        [TestCase(LogLevel.Error, true)]
        [TestCase(LogLevel.Critical, true)]
        public void HasFailed_FailureDetection_FailuresAreDetected(LogLevel logLevel, bool hasFailed)
        {
            var logger = new InMemLogger();
            logger.Log(new LogEntry(DateTime.UtcNow, "message", logLevel));

            logger.Failed.Should().Be(hasFailed, logLevel.ToName());
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Trace)]
        [TestCase(LogLevel.Information)]
        [TestCase(LogLevel.Warning)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Critical)]
        public void Log_Entries_EntryIsLogged(LogLevel logLevel)
        {
            var logger = new InMemLogger();
            logger.Count.Should().Be(0);
            logger.Get(logLevel).Count.Should().Be(0);
            logger.Log(new LogEntry(DateTime.UtcNow, "message", logLevel));
            logger.Get(logLevel).Count.Should().Be(1);
            logger.Count.Should().Be(1);
        }

        [Test]
        public void Error_LogException_ExceptionIsFormattedNicely()
        {
            var logger = new InMemLogger();

            var innerEx = new IOException("Im Inner Exception");
            var ex = new ApplicationException("Im outer exception", innerEx);
            var timeStamp = new DateTime(2000, 01, 01, 01, 01, 01);
            logger.Log(timeStamp, null, LogLevel.Error, ex);

            var entry = logger.Single(e => e.Level == LogLevel.Error);

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
            var output = logger.Single(e => e.Level == LogLevel.Error).Message;

            output.Should().Be($"[2000-01-01 01:01:01] <Error> {message}\r\nSystem.ApplicationException: Im outer exception ---> System.IO.IOException: Im Inner Exception\r\n   --- End of inner exception stack trace ---");
        }

        [Test]
        public void Add_FilterTypes_TypesAreFiltered()
        {
            var logger = new InMemLogger();

            logger.LogDebug("debug");
            logger.LogTrace("trace");
            logger.LogInformation("info");
            logger.LogWarning("warning");
            logger.LogError("error");
            logger.LogCritical("critical");

            logger.Count.Should().Be(6);
            logger.Get(LogLevel.Debug).Count.Should().Be(1);
            logger.Get(LogLevel.Trace).Count.Should().Be(1);
            logger.Get(LogLevel.Information).Count.Should().Be(1);
            logger.Get(LogLevel.Warning).Count.Should().Be(1);
            logger.Get(LogLevel.Error).Count.Should().Be(1);
            logger.Get(LogLevel.Critical).Count.Should().Be(1);
        }
    }
}
