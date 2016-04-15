using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Diagnostics
{
    [TestFixture]
    public class MediatorDiagnosticsTests
    {
        [Test]
        public void Log_InMemDiagnostics_AllLoggersAreLoggedTo()
        {
            var runs = Enumerable.Range(0, 7); //random prime
            var mediator = new MediatorDiagnostics();
            var loggers = new List<InMemDiagnostics>();
            var logLevel = LogLevel.Debug;
            runs.ForEach(i => loggers.Add(new InMemDiagnostics())); //add loggers
            runs.ForEach(i => mediator.Add(i.ToString(), loggers[i])); //add loggers

            loggers.ForEach(l => l.GetLogs(logLevel).Count.Should().Be(0));//ensure loggers are empty

            //act
            runs.ForEach(i => mediator.Log(i.ToString(), logLevel));//log

            //assert
            loggers.ForEach(l => l.GetLogs(logLevel).Count.Should().Be(runs.Count()));
        }
    }
}
