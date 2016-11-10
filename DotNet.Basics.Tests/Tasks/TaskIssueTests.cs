using System;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class TaskIssueTests
    {
        [Fact]
        public void Ctor_Message_MessageIsSet()
        {
            var msg = "Ctor_Message_MessageIsSet";
            //act
            var issue = new TaskIssue(msg);

            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeNull();
        }

        [Fact]
        public void Ctor_Exception_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_Exception_MessageAndExceptionAreSet";
            var ex = new ApplicationException(msg);
            
            //act
            var issue = new TaskIssue(ex);

            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeOfType<ApplicationException>();
        }

        [Fact]
        public void Ctor_MessageAndException_MessageAndExceptionAreSet()
        {
            var msg = "Ctor_MessageAndException_MessageAndExceptionAreSet";
            var exMsg = "sd rtlæhd rglid hg klud hglid hgldfui";
            var ex = new ApplicationException(exMsg);

            //act
            var issue = new TaskIssue(msg, ex);

            issue.Message.Should().Be(msg);
            issue.Exception.Should().BeOfType<ApplicationException>();
            issue.Exception.Message.Should().Be(exMsg);
        }
    }
}
