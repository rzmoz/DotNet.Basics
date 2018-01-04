﻿using System;
using DotNet.Standard.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Standard.Tests.Tasks
{
    public class TaskIssueTests
    {
        [Fact]
        public void Ctor_Message_MessageIsSet()
        {
            var msg = "Ctor_Message_MessageIsSet";
            //act
            var issue = new TaskIssue(LogLevel.Error, msg);

            issue.LogLevel.Should().Be(LogLevel.Error);
            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeNull();
        }

        [Fact]
        public void HasException_Null_False()
        {
            //act
            var issue = new TaskIssue(LogLevel.Error, "HasException_Null_False", null);

            issue.LogLevel.Should().Be(LogLevel.Error);
            issue.HasException.Should().BeFalse();
        }
        [Fact]
        public void HasException_ExceptionNotNull_True()
        {
            //act
            var issue = new TaskIssue(LogLevel.Error, "HasException_ExceptionNotNull_True", new ApplicationException());

            issue.LogLevel.Should().Be(LogLevel.Error);
            issue.HasException.Should().BeTrue();
        }

        [Fact]
        public void Ctor_Exception_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_Exception_MessageAndExceptionAreSet";
            var ex = new ArithmeticException(msg);

            //act
            var issue = new TaskIssue(LogLevel.Error, ex.Message, ex);

            issue.LogLevel.Should().Be(LogLevel.Error);
            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeOfType<ArithmeticException>();
        }

        [Fact]
        public void Ctor_MessageAndException_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_MessageAndException_MessageAndExceptionAreSet";
            var exMsg = "sd rtlæhd rglid hg klud hglid hgldfui";
            var ex = new ArithmeticException(exMsg);

            //act
            var issue = new TaskIssue(LogLevel.Error, msg, ex);

            issue.LogLevel.Should().Be(LogLevel.Error);
            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeOfType<ArithmeticException>();
            issue.Exception.Message.Should().Be(exMsg);
        }
    }
}