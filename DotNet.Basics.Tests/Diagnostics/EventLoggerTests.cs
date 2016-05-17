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
            string expectedMessage = "MyMessage";
            string actualMessage = null;
            
            logger.EntryLogged += (o, e) => { actualMessage = e.Message; };

            actualMessage.Should().NotBe(expectedMessage);

            logger.LogInformation("MyMessage");

            actualMessage.Should().Be(expectedMessage);
        }
    }
}
