using DotNet.Basics.NLog;
using FluentAssertions;
using NLog.LogReceiverService;
using Xunit;

namespace DotNet.Basics.Tests.NLog
{
    public class NLogExtensions
    {
        [Fact]
        public void NLog_ImplicitName_LoggerNameIsResolved()
        {
            var logger = this.NLog();

            logger.Name.Should().Be(typeof(NLogExtensions).FullName);
        }

        [Fact]
        public void NLog_ExplicitName_LoggerNameIsResolved()
        {
            var loggerName = "NLog_ExplicitName_LoggerNameIsResolvedProperly";

            var logger = this.NLog(loggerName);

            logger.Name.Should().Be(loggerName);
        }
        [Fact]
        public void NLog_Generic_LoggerNameIsResolved()
        {
            var logger = this.NLog<NLogEvent>();

            logger.Name.Should().Be(typeof(NLogEvent).FullName);
        }

        [Fact]
        public void NLog_Type_LoggerNameIsResolved()
        {
            var @event = new NLogEvent();

            var logger = this.NLog(@event.GetType());

            logger.Name.Should().Be(typeof(NLogEvent).FullName);
        }
    }
}
