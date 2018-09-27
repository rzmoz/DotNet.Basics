using System;
using System.IO;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.Diagnostics
{
    public class TaskResultTests
    {
        [Fact]
        public void Log_DefaultCtor_NoEntries()
        {
            //act
            var result = new TaskResult();

            result.Log.None().Should().BeTrue();
            result.Log.Any().Should().BeFalse();
        }

        [Fact]
        public void Log_Enqueue_EntriesFound()
        {
            var entry = "Log_Enqueue_EntriesFound";

            //act
            var result = new TaskResult(log =>
            {
                log.Add(LogLevel.Error, entry);
            });

            result.Log.None().Should().BeFalse();
            result.Log.Single().Message.Should().Be(entry);
        }

        [Fact]
        public void Log_Enqueue_NewEntriesAreAppended()
        {
            var entry = "Log_Enqueue_NewEntriesAreAppended";

            var initialEntry = entry + "Initial";

            //act
            var initialResult = new TaskResult(log =>
            {
                log.Add(LogLevel.Error, initialEntry, new IOException());
            });

            initialResult.Log.None().Should().BeFalse();
            initialResult.Log.Single().Message.Should().Be(initialEntry);
            initialResult.Log.Single().Exception.Should().BeOfType<IOException>();

            var appendedEntry = entry + "Appended";

            var joinedResult = initialResult.Append(log =>
            {
                log.Add(LogLevel.Critical, appendedEntry, new ArgumentException());
            });

            joinedResult.Log.None().Should().BeFalse();
            joinedResult.Log.Count.Should().Be(2);
            joinedResult.Log.First().LogLevel.Should().Be(LogLevel.Error);
            joinedResult.Log.First().Message.Should().Be(initialEntry);
            joinedResult.Log.First().Exception.Should().BeOfType<IOException>();

            joinedResult.Log.Last().LogLevel.Should().Be(LogLevel.Critical);
            joinedResult.Log.Last().Message.Should().Be(appendedEntry);
            joinedResult.Log.Last().Exception.Should().BeOfType<ArgumentException>();
        }
    }
}
