using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class SemVersionPreReleaseTests
    {
        [Fact]
        public void PreRelease_Precedence_VersionsAreOrderedByPreReleaseAlphabetically()
        {
            var alpha1 = new SemVersionPreRelease("Alpha.1");
            var alpha2 = new SemVersionPreRelease("ALPHA.2");
            var beta1 = new SemVersionPreRelease("beta.1");
            var rc = new SemVersionPreRelease("rc");

            //smaller than - true
            (alpha1 < alpha2).Should().BeTrue();
            (alpha1 < beta1).Should().BeTrue();
            (alpha1 < rc).Should().BeTrue();
            (alpha2 < beta1).Should().BeTrue();
            (alpha2 < rc).Should().BeTrue();

            //smaller than - false
#pragma warning disable 1718
            (alpha1 < alpha1).Should().BeFalse();
            (alpha2 < alpha1).Should().BeFalse();
            (beta1 < alpha1).Should().BeFalse();
            (rc < alpha1).Should().BeFalse();
            (alpha2 < alpha2).Should().BeFalse();
            (beta1 < alpha2).Should().BeFalse();
            (rc < alpha2).Should().BeFalse();
            (beta1 < beta1).Should().BeFalse();
            (rc < beta1).Should().BeFalse();
            (rc < rc).Should().BeFalse();

            //larger than - true
            (alpha2 > alpha1).Should().BeTrue();
            (beta1 > alpha1).Should().BeTrue();
            (rc > alpha1).Should().BeTrue();
            (beta1 > alpha2).Should().BeTrue();
            (rc > alpha2).Should().BeTrue();
            (rc > beta1).Should().BeTrue();

            //larger than - false
            (alpha1 > alpha1).Should().BeFalse();

            (alpha1 > alpha2).Should().BeFalse();
            (alpha1 > beta1).Should().BeFalse();
            (alpha1 > rc).Should().BeFalse();
            (alpha2 > alpha2).Should().BeFalse();
            (alpha2 > beta1).Should().BeFalse();
            (alpha2 > rc).Should().BeFalse();
            (beta1 > beta1).Should().BeFalse();
            (beta1 > rc).Should().BeFalse();
            (rc > rc).Should().BeFalse();
        }

        [Fact]
        public void Comparison_Empty_SomePreReleaseIsAlwaysLowerThanNoPreRelease()
        {
            var somePr = new SemVersionPreRelease("alpha.1");
            var emptyPr = new SemVersionPreRelease();

            (emptyPr > somePr).Should().BeTrue();
            (somePr < emptyPr).Should().BeTrue();
        }

        [Fact]
        public void Ctor_Parse_SeparatorIsIdentifiedWhenParsed()
        {
            var pr = new SemVersionPreRelease("alpha.1");
            pr.Identifiers.Any(i => i.Identifier.Contains(".")).Should().BeFalse();
        }

        [Fact]// §11 Precedence in https://semver.org/
        public void Precedence_FullExampleFromSemVerDotOrg_VersionsAreInOrder()
        {
            var preRelease1 = new SemVersionPreRelease("alpha");
            var preRelease2 = new SemVersionPreRelease("alpha.1");
            var preRelease3 = new SemVersionPreRelease("alpha.beta");
            var preRelease4 = new SemVersionPreRelease("beta");
            var preRelease5 = new SemVersionPreRelease("beta.2");
            var preRelease6 = new SemVersionPreRelease("beta.11");
            var preRelease7 = new SemVersionPreRelease("rc.1");

            (preRelease1 < preRelease2).Should().BeTrue();
            (preRelease1 < preRelease3).Should().BeTrue();
            (preRelease1 < preRelease4).Should().BeTrue();
            (preRelease1 < preRelease5).Should().BeTrue();
            (preRelease1 < preRelease6).Should().BeTrue();
            (preRelease1 < preRelease7).Should().BeTrue();

            (preRelease2 < preRelease3).Should().BeTrue();
            (preRelease2 < preRelease4).Should().BeTrue();
            (preRelease2 < preRelease5).Should().BeTrue();
            (preRelease2 < preRelease6).Should().BeTrue();
            (preRelease2 < preRelease7).Should().BeTrue();

            (preRelease3 < preRelease4).Should().BeTrue();
            (preRelease3 < preRelease5).Should().BeTrue();
            (preRelease3 < preRelease6).Should().BeTrue();
            (preRelease3 < preRelease7).Should().BeTrue();

            (preRelease4 < preRelease5).Should().BeTrue();
            (preRelease4 < preRelease6).Should().BeTrue();
            (preRelease4 < preRelease7).Should().BeTrue();

            (preRelease5 < preRelease6).Should().BeTrue();
            (preRelease5 < preRelease7).Should().BeTrue();

            (preRelease6 < preRelease7).Should().BeTrue();

            //////////////////////////////////////
            (preRelease2 > preRelease1).Should().BeTrue();
            (preRelease3 > preRelease1).Should().BeTrue();
            (preRelease4 > preRelease1).Should().BeTrue();
            (preRelease5 > preRelease1).Should().BeTrue();
            (preRelease6 > preRelease1).Should().BeTrue();
            (preRelease7 > preRelease1).Should().BeTrue();

            (preRelease3 > preRelease2).Should().BeTrue();
            (preRelease4 > preRelease2).Should().BeTrue();
            (preRelease5 > preRelease2).Should().BeTrue();
            (preRelease6 > preRelease2).Should().BeTrue();
            (preRelease7 > preRelease2).Should().BeTrue();

            (preRelease4 > preRelease3).Should().BeTrue();
            (preRelease5 > preRelease3).Should().BeTrue();
            (preRelease6 > preRelease3).Should().BeTrue();
            (preRelease7 > preRelease3).Should().BeTrue();

            (preRelease5 > preRelease4).Should().BeTrue();
            (preRelease6 > preRelease4).Should().BeTrue();
            (preRelease7 > preRelease4).Should().BeTrue();

            (preRelease6 > preRelease5).Should().BeTrue();
            (preRelease7 > preRelease5).Should().BeTrue();

            (preRelease7 > preRelease6).Should().BeTrue();
        }
    }
}
