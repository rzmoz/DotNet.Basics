using System;
using DotNet.Basics.Pipelines;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class TaskPipelineResultTests
    {
        [Test]
        public void Ctor_NoProfile_DureationIsZero()
        {
            var result = new TaskPipelineResult<EventArgs>();

            result.Profile.Duration.Should().Be(0.Seconds());
        }
    }
}
