using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class SemVersionTests
    {
        [Fact]
        public void PreRelease_Precedence_PreReleaseIsIgnoredWhenAnyMajorMinorOrPatchIsDifferent()
        {
            var preRelease = "rc.1";
            var higherWithPreRelease = new SemVersion(1, 0, 2, preRelease);
            var lowerWithPreRelease = new SemVersion(1, 0, 0, preRelease);

            //smaller than
            (lowerWithPreRelease < higherWithPreRelease).Should().BeTrue();

            //larger than
            (lowerWithPreRelease > higherWithPreRelease).Should().BeFalse();
        }

        [Fact]
        public void PreRelease_Precedence_AnyPreReleaseTakesLowerPrecedence()
        {
            var woPreRelease = new SemVersion(1, 0, 0);
            var withPreRelease = new SemVersion(1, 0, 0, "rc.1");

            //smaller than
            (withPreRelease < woPreRelease).Should().BeTrue();

            //larger than
            (withPreRelease > woPreRelease).Should().BeFalse();
        }

        [Fact]
        public void MajorMinorPatch_Precedence_HigherVersionTakesPrecedence()
        {
            var order1 = new SemVersion(1, 0, 0);
            var order2 = new SemVersion(2, 0, 0);
            var order3 = new SemVersion(2, 1, 0);
            var order4 = new SemVersion(2, 1, 1);

            //smaller than
            (order1 < order2).Should().BeTrue();
            (order1 < order3).Should().BeTrue();
            (order1 < order4).Should().BeTrue();
            (order2 < order3).Should().BeTrue();
            (order2 < order4).Should().BeTrue();
            (order3 < order4).Should().BeTrue();

            //larger than
            (order1 > order2).Should().BeFalse();
            (order1 > order3).Should().BeFalse();
            (order1 > order4).Should().BeFalse();
            (order2 > order3).Should().BeFalse();
            (order2 > order4).Should().BeFalse();
            (order3 > order4).Should().BeFalse();
        }
    }
}
