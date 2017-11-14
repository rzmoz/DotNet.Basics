using System;
using System.IO;
using System.Linq;
using DotNet.Basics.Tasks;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class TaskResultTests
    {
        [Fact]
        public void Issues_DefaultCtor_NoIssues()
        {
            //act
            var result = new TaskResult();

            result.Issues.None().Should().BeTrue();
            result.Issues.Any().Should().BeFalse();
        }

        [Fact]
        public void Issues_99_ButaBAintOne()
        {
            var inputCount = 98;

            //act
            var result = new TaskResult(issues => Enumerable.Range(1, inputCount).ForEach(i => issues.Add(i.ToString())));

            result.Issues.Count.Should().Be(inputCount + 1);
            result.Issues.Last().Message.Should().Be("I got 99 issues but a b**** ain't one");
        }

        [Fact]
        public void Issues_Add_IssuesFound()
        {
            var issueMessage = "Ctor_WithIssue_IssuesFound";

            //act
            var result = new TaskResult(issues =>
            {
                issues.Add(issueMessage);
            });

            result.Issues.None().Should().BeFalse();
            result.Issues.Single().Message.Should().Be(issueMessage);
        }

        [Fact]
        public void Issues_Append_NewIssuesAreAppended()
        {
            var issueMessage = "Ctor_Append_NewIssuesAreAppended_";

            var initialIssueMessage = issueMessage + "Initial";

            //act
            var initialResult = new TaskResult(issues =>
            {
                issues.Add(initialIssueMessage, new IOException());
            });

            initialResult.Issues.None().Should().BeFalse();
            initialResult.Issues.Single().Message.Should().Be(initialIssueMessage);
            initialResult.Issues.Single().Exception.Should().BeOfType<IOException>();

            var appendedIssueMessage = issueMessage + "Appended";

            var joinedResult = initialResult.Append(issues =>
            {
                issues.Add(appendedIssueMessage, new ArgumentException());
            });

            joinedResult.Issues.None().Should().BeFalse();
            joinedResult.Issues.Count.Should().Be(2);
            joinedResult.Issues.First().Message.Should().Be(initialIssueMessage);
            joinedResult.Issues.First().Exception.Should().BeOfType<IOException>();

            joinedResult.Issues.Last().Message.Should().Be(appendedIssueMessage);
            joinedResult.Issues.Last().Exception.Should().BeOfType<ArgumentException>();
        }
    }
}
