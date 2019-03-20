using System;
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
        private static readonly string _metaData = "sdfkjsh.fs.jkhf++++sdjkhf";
        private static readonly string _semver10String = $"{_major}.{_minor}.{_patch}";
        private static readonly string _fullSemver20String = $"{_major}.{_minor}.{_patch}-{_preRelease}+{_metaData}";


        [Theory]
        [InlineData("1.0.5-rc.1+555")]//full string
        [InlineData("1.0.5+555")]//wo prerelease
        [InlineData("1.0.5-rc.111")]//wo metadata
        public void ToSemver20String_ToString_StringIsFormatted(string semVer20String)
        {
            //act
            var semVer = SemVersion.Parse(semVer20String);

            //assert
            semVer.SemVer20String.Should().Be(semVer20String);
        }

        [Fact]
        public void Parse_SemVer10_VersionIsParsed()
        {
            //act
            var semVer = SemVersion.Parse(_semver10String);

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().BeEmpty();
            semVer.Metadata.Should().BeEmpty();
        }

        [Fact]
        public void Parse_SemVer20WoPreReleaseWithMetadata_VersionIsParsed()
        {
            //act
            var semVer = SemVersion.Parse($"{_semver10String}+{_metaData}");

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().BeEmpty();
            semVer.Metadata.Should().Be(_metaData);
        }

        [Fact]
        public void Parse_FullSemVer20_VersionIsParsed()
        {
            //act
            var semVer = SemVersion.Parse(_fullSemver20String);

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.Should().Be(_preRelease);
            semVer.Metadata.Should().Be(_metaData);
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

        [Fact(Skip = "TODO")]
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
