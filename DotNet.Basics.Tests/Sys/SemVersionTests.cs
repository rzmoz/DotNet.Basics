using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class SemVersionTests
    {

        private static readonly int _major = 10;
        private static readonly int _minor = 701;
        private static readonly int _patch = 232;
        private static readonly string _preRelease = "rc.1";
        private static readonly string _metaData = "sdfkjsh.fs.jkhf++djkhf";
        private static readonly string _semver10String = $"{_major}.{_minor}.{_patch}";
        private static readonly string _fullSemver20String = $"{_major}.{_minor}.{_patch}-{_preRelease}+{_metaData}";

        [Theory]
        [InlineData("1.0.5-rc.1+555")]//full string
        [InlineData("1.0.5+555")]//wo preRelease
        [InlineData("1.0.5-rc.111")]//wo metadata
        public void ToSemver20String_ToString_StringIsFormatted(string semVer20String)
        {
            //act
            var semVer = new SemVersion(semVer20String);

            //assert
            semVer.SemVer20String.Should().Be(semVer20String);
        }

        [Fact]
        public void Metadata_Set_ToSemverStringReflectsChangesToSemVerStrings()
        {
            var semVer = new SemVersion(_fullSemver20String);
            var before = semVer.SemVer20String;
            //act
            semVer.Metadata = "Something.else";

            //assert
            semVer.SemVer20String.Should().NotBe(before);

        }
        [Fact]
        public void Metadata_UpdateBlank_MetadataIsUpdated()
        {
            var metadata = "Something.else";
            var semVer = new SemVersion(1, 1, 1);

            //act
            semVer.Metadata += metadata;

            //assert
            semVer.SemVer20String.Should().Be($"1.1.1+{metadata }");
        }

        [Fact]
        public void Ctor_Object_ObjectIsParsedAsString()
        {
            //act
            var semVer = new SemVersion((object)_semver10String);//no exception is thrown

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().BeEmpty();
            semVer.Metadata.Should().BeEmpty();
        }
        [Fact]
        public void Ctor_SemVer10_VersionIsParsed()
        {
            //act
            var semVer = new SemVersion(_semver10String);

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().BeEmpty();
            semVer.Metadata.Should().BeEmpty();
        }

        [Fact]
        public void Ctor_SemVer20WoPreReleaseWithMetadata_VersionIsParsed()
        {
            //act
            var semVer = new SemVersion($"{_semver10String}+{_metaData}");

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().BeEmpty();
            semVer.Metadata.Should().Be(_metaData);
        }

        [Fact]
        public void Ctor_FullSemVer20_VersionIsParsed()
        {
            //act
            var semVer = new SemVersion(_fullSemver20String);

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().Be(_preRelease);
            semVer.Metadata.Should().Be(_metaData);
        }

        [Theory]
        [InlineData("v1")]
        [InlineData("V1")]
        public void Ctor_vInVersionString_VersionIsParsed(string semVerWithV)
        {
            //act
            var semVer = new SemVersion(semVerWithV);

            //assert
            semVer.Major.Should().Be(1);
        }

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
        public void PreRelease_Precedence_VersionsAreOrderedByPreReleaseAlphabetically()
        {
            var alpha1 = new SemVersion(1, 0, 0, "Alpha.1");
            var alpha2 = new SemVersion(1, 0, 0, "ALPHA.2");
            var beta1 = new SemVersion(1, 0, 0, "beta.1");
            var rc = new SemVersion(1, 0, 0, "rc");

            //smaller than - true
            (alpha1 < alpha2).Should().BeTrue();
            (alpha1 < beta1).Should().BeTrue();
            (alpha1 < rc).Should().BeTrue();
            (alpha2 < beta1).Should().BeTrue();
            (alpha2 < rc).Should().BeTrue();

            //smaller than - false
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
        public void PreRelease_Precedence_AnyPreReleaseTakesLowerPrecedence()
        {
            var woPreRelease = new SemVersion(1, 0, 0);
            var withPreRelease = new SemVersion(1, 0, 0, "rc.1");

            //smaller than
            (withPreRelease < woPreRelease).Should().BeTrue();

            //equals
            (withPreRelease == woPreRelease).Should().BeFalse();
            (withPreRelease != woPreRelease).Should().BeTrue();

            //larger than
            (withPreRelease > woPreRelease).Should().BeFalse();
        }

        [Fact]
        public void MaxMin_Equality_VersionsCanBeOrderedInCollection()
        {
            var min = new SemVersion(1, 0, 0);
            var middle = new SemVersion(2, 1, 0);
            var max = new SemVersion(3, 0, 0);

            var list = new List<SemVersion> { min, middle, max };

            list.Min().Should().Be(min);
            list.Max().Should().Be(max);

            list.OrderBy(v => v).First().Should().Be(min);
            list.OrderBy(v => v).Last().Should().Be(max);
        }

        [Fact]
        public void Equality_Compare_VersionsCanBeCompared()
        {
            var ver1 = new SemVersion(_fullSemver20String);
            var sameAsVer1 = new SemVersion(_fullSemver20String);
            var ver2 = new SemVersion(2, 0, 1);

            (ver1 == sameAsVer1).Should().BeTrue();
            ver1.Equals(sameAsVer1).Should().BeTrue();

            (ver2 == ver1).Should().BeFalse();
        }

        [Fact]
        public void MajorMinorPatch_Precedence_HigherVersionTakesPrecedence()
        {
            var order1 = new SemVersion(1, 0, 0);
            var order2 = new SemVersion(2, 0, 0);
            var order3 = new SemVersion(2, 1, 0);
            var order4 = new SemVersion(2, 1, 1);
            var order5 = new SemVersion(3, 0, 0);

            //smaller than - true
            (order1 < order2).Should().BeTrue();
            (order1 < order3).Should().BeTrue();
            (order1 < order4).Should().BeTrue();
            (order1 < order5).Should().BeTrue();
            (order2 < order3).Should().BeTrue();
            (order2 < order4).Should().BeTrue();
            (order2 < order5).Should().BeTrue();
            (order3 < order4).Should().BeTrue();
            (order3 < order5).Should().BeTrue();
            (order4 < order5).Should().BeTrue();

            //smaller than - false
            (order1 < order1).Should().BeFalse();
            (order2 < order1).Should().BeFalse();
            (order3 < order1).Should().BeFalse();
            (order4 < order1).Should().BeFalse();
            (order5 < order1).Should().BeFalse();
            (order2 < order2).Should().BeFalse();
            (order3 < order2).Should().BeFalse();
            (order4 < order2).Should().BeFalse();
            (order5 < order2).Should().BeFalse();
            (order3 < order3).Should().BeFalse();
            (order4 < order3).Should().BeFalse();
            (order5 < order3).Should().BeFalse();
            (order4 < order4).Should().BeFalse();
            (order5 < order4).Should().BeFalse();
            (order5 < order5).Should().BeFalse();

            //larger than - true
            (order2 > order1).Should().BeTrue();
            (order3 > order1).Should().BeTrue();
            (order4 > order1).Should().BeTrue();
            (order5 > order1).Should().BeTrue();
            (order3 > order2).Should().BeTrue();
            (order4 > order2).Should().BeTrue();
            (order5 > order2).Should().BeTrue();
            (order4 > order3).Should().BeTrue();
            (order5 > order3).Should().BeTrue();
            (order5 > order4).Should().BeTrue();

            //larger than - false
            (order1 > order1).Should().BeFalse();
            (order1 > order2).Should().BeFalse();
            (order2 > order2).Should().BeFalse();
            (order1 > order3).Should().BeFalse();
            (order1 > order4).Should().BeFalse();
            (order2 > order3).Should().BeFalse();
            (order3 > order3).Should().BeFalse();
            (order2 > order4).Should().BeFalse();
            (order3 > order4).Should().BeFalse();
            (order4 > order4).Should().BeFalse();
            (order1 > order5).Should().BeFalse();
            (order2 > order5).Should().BeFalse();
            (order3 > order5).Should().BeFalse();
            (order4 > order5).Should().BeFalse();
            (order5 > order5).Should().BeFalse();
        }
    }
}

