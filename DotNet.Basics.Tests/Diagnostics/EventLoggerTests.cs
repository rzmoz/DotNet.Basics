using DotNet.Basics.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class EventLoggerTests
    {
        [Test]
        public void Log_Events_AllEventsAreFired()
        {
            var logger = new EventLogger();
            string logMessage = null;
            string rawLogMessage = string.Empty;//something different than logMessage

            logger.EntryLogged += (o, e) => { logMessage = e.Message; };

            logMessage.Should().NotBe(rawLogMessage);

            logger.LogInformation("MyMessage");

            logMessage.Should().Be(rawLogMessage);
        }
    }
}
