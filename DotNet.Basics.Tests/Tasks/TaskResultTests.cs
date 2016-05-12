using System;
using System.Collections.Generic;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class TaskResultTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finished_Set_IsSet(bool finished)
        {
            var result = new TaskResult(finished);

            result.Finished.Should().Be(finished);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("MyResult")]
        public void Finished_Set_IsSet(string name)
        {
            var result = new TaskResult(name:name);

            result.Name.Should().Be(name??string.Empty);
        }


        [Test]
        [TestCase(LogLevel.Debug, false)]
        [TestCase(LogLevel.Verbose, false)]
        [TestCase(LogLevel.Information, false)]
        [TestCase(LogLevel.Warning, false)]
        [TestCase(LogLevel.Error, true)]
        [TestCase(LogLevel.Critical, true)]
        public void HasFailed_FailureDetection_FailuresAreDetected(LogLevel logLevel, bool hasFailed)
        {
            var logger = new InMemLogger();
            logger.Log(new LogEntry(DateTime.UtcNow, "message", logLevel));

            var result = new TaskResult(logger.Entries);

            result.HasFailed.Should().Be(hasFailed, logLevel.ToName());
        }

        [Test]
        public void Debugs_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Debugs, LogLevel.Debug);
        }
        [Test]
        public void Verboses_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Verboses, LogLevel.Verbose);
        }
        [Test]
        public void Informations_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Informations, LogLevel.Information);
        }
        [Test]
        public void Warnings_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Warnings, LogLevel.Warning);
        }
        [Test]
        public void Errors_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Errors, LogLevel.Error);
        }
        [Test]
        public void Criticalss_ResultInclusion_IsDetected()
        {
            AssertLogEntryFunc(result => result.Criticals, LogLevel.Critical);
        }

        private void AssertLogEntryFunc(Func<TaskResult, IReadOnlyCollection<LogEntry>> getCount, LogLevel logLevel)
        {
            var logger = new InMemLogger();
            logger.Log(new LogEntry(DateTime.UtcNow, "message", logLevel));

            var result = new TaskResult(logger.Entries);

            getCount(result).Count.Should().Be(1);

            result.GetLogs(logLevel).Count.Should().Be(1);
            
        }
    }
}
