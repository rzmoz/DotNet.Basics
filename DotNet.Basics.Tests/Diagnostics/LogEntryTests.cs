using System;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.Diagnostics
{
    public class LogEntryTests
    {
        [Fact]
        public void Ctor_Message_MessageIsSet()
        {
            var msg = "Ctor_Message_MessageIsSet";
            //act
            var entry = new LogEntry(LogLevel.Error, msg);

            entry.LogLevel.Should().Be(LogLevel.Error);
            entry.Message.Should().Be(msg);
            entry.Exception.Should().BeNull();
        }

        [Fact]
        public void HasException_Null_False()
        {
            //act
            var entry = new LogEntry(LogLevel.Error, "HasException_Null_False", null);

            entry.LogLevel.Should().Be(LogLevel.Error);
            entry.HasException.Should().BeFalse();
        }
        [Fact]
        public void HasException_ExceptionNotNull_True()
        {
            //act
            var entry = new LogEntry(LogLevel.Error, "HasException_ExceptionNotNull_True", new ApplicationException());

            entry.LogLevel.Should().Be(LogLevel.Error);
            entry.HasException.Should().BeTrue();
        }

        [Fact]
        public void Ctor_Exception_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_Exception_MessageAndExceptionAreSet";
            var ex = new ArithmeticException(msg);

            //act
            var entry = new LogEntry(LogLevel.Error, ex.Message, ex);

            entry.LogLevel.Should().Be(LogLevel.Error);
            entry.Message.Should().Be(msg);
            entry.Exception.Should().BeOfType<ArithmeticException>();
        }

        [Fact]
        public void Ctor_MessageAndException_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_MessageAndException_MessageAndExceptionAreSet";
            var exMsg = "sd rtlæhd rglid hg klud hglid hgldfui";
            var ex = new ArithmeticException(exMsg);

            //act
            var entry = new LogEntry(LogLevel.Error, msg, ex);

            entry.LogLevel.Should().Be(LogLevel.Error);
            entry.Message.Should().Be(msg);
            entry.Exception.Should().BeOfType<ArithmeticException>();
            entry.Exception.Message.Should().Be(exMsg);
        }
    }
}
