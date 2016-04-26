using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class MediatorLoggersTests
    {
        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Verbose)]
        [TestCase(LogLevel.Information)]
        [TestCase(LogLevel.Warning)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Critical)]
        public void Log_MediatorLogging_AllLoggersAreLoggedTo(LogLevel logLevel)
        {
            var runs = Enumerable.Range(0, 7).ToList(); //random prime
            var mediator = new MediatorLogger();
            var loggers = new List<InMemLogger>();
            runs.ForEach(i => loggers.Add(new InMemLogger())); //add loggers
            runs.ForEach(i => mediator.Add(i.ToString(), loggers[i])); //add loggers

            loggers.ForEach(l => l.GetLogs(logLevel).Count.Should().Be(0));//ensure loggers are empty

            //act
            runs.ForEach(i => mediator.Log(i.ToString(), logLevel));//log

            //assert
            loggers.ForEach(l => l.GetLogs(logLevel).Count.Should().Be(runs.Count()));
        }
    }
}
