using DotNet.Basics.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Collections
{
    [TestFixture]
    public class CollectionExtensions
    {
        [Test]
        public void ForEach_SimpleAction_ActionIsAppliedToallElementsInCol()
        {
            const int expected = 2;

            var ones = new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

            var twos = ones.ForEach(one => one = expected);

            foreach (var two in twos)
                two.Should().Be(expected);
        }
    }
}
