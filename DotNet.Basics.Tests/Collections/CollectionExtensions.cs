using System.Linq;
using DotNet.Basics.Collections;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Collections
{
    [TestFixture]
    public class CollectionExtensions
    {
        [Test]
        public void ForEach_Func_ActionIsAppliedToallElementsInCol()
        {
            const int expected = 2;

            var ones = new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            var twos = ones.ForEach(one => expected);

            foreach (var two in twos)
                two.Should().Be(expected);
        }

        [Test]
        public void ForEach_Action_ActionIsAppliedToallElementsInCol()
        {
            var range = Enumerable.Range(1, 10).ToList();
            var test = Substitute.For<ITest>();

            range.ForEach(one => test.Test());

            test.Received(range.Count());
        }

        public interface ITest
        {
            void Test();
        }
    }
}
