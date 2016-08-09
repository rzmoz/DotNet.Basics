using DotNet.Basics.Pipelines;
using FluentAssertions;
using NLog;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineLoggerTests
    {
        [Test]
        public void Log_Events_AllEventsAreFired()
        {
            var logger = new PipelineLogger();
            string expectedMessage = "MyMessage";
            
            foreach (var level in LogLevel.AllLoggingLevels)
            {
                string actualMessage = null;
                logger.EntryLogged += (o, e) => { actualMessage = e.Message; };
                logger.Log(level, "MyMessage");
                actualMessage.Should().Be(expectedMessage);
            }
        }
    }
}
