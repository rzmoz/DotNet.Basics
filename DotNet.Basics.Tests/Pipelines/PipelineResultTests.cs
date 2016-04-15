using System;
using DotNet.Basics.Pipelines;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineResultTests
    {
        [Test]
        public void Ctor_NoArgs_Ar()
        {
            var result = new PipelineResult<EventArgs>();

            result.Args.Should().NotBeNull();
            result.Args.Should().BeOfType(typeof(EventArgs));
        }
    }
}
