using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class SemVersionTests
    {
        private const int _major = 10;
        private const int _minor = 701;
        private const int _patch = 232;
        private const string _preRelease = "rc.1";
        private const string _metaData = "sdfkjsh.fs.jkhf++djkhf";
        private const string _fileVerString = "10.701.232";
        private const string _semver10String = "10.701.232-beta.25.43";
        private const string _fullSemver20String = _fileVerString + "-" + _preRelease + "+" + _metaData;

        [Fact]
        public void Serialize_SystemTextJson_IsSerialized()
        {

            var obj = new SemVersion(_fullSemver20String);

            Action action = () => System.Text.Json.JsonSerializer.Serialize(obj);

            action.Should().NotThrow();
        }

        [Fact]
        public void Serialize_NewtonSoft_IsSerialized()
        {
            var obj = new SemVersion(_fullSemver20String);
            string jsonStr;
            Action action = () => jsonStr = JsonConvert.SerializeObject(obj);

            action.Should().NotThrow();
        }

        [Fact]
        public void ToSemVersion_Construction_SemVersionIsConstructed()
        {
            //act
            var semVer = _fullSemver20String.ToSemVersion();

            //assert
            semVer.SemVer20String.Should().Be(_fullSemver20String);
        }

        [Fact]
        public void Deserialization_Newtonsoft_StringIsDeserialized()
        {
            var jsonStr = @"{""Major"":10,""Minor"":701,""Patch"":232,""PreRelease"":{""Identifiers"":[{""Identifier"":""rc"",""IsNumeric"":false},{""Identifier"":""1"",""IsNumeric"":true}]},""Metadata"":""sdfkjsh.fs.jkhf++djkhf"",""SemVer10String"":""10.701.232"",""SemVer20String"":""10.701.232-rc.1+sdfkjsh.fs.jkhf++djkhf""}";
            var obj = JsonConvert.DeserializeObject<SemVersion>(jsonStr);
            obj.SemVer20String.Should().Be(_fullSemver20String);
        }


        [Theory]
        [InlineData(_fullSemver20String)]//full string
        [InlineData("1.0.5+555")]//wo preRelease
        [InlineData("1.0.5-rc.111")]//wo metadata
        public void ToSemver20String_ToString_StringIsFormatted(string semVer20String)
        {
            //act
            var semVer = semVer20String.ToSemVersion();

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
            semVer.SemVer20String.Should().Be($"1.1.1+{metadata}");
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
            semVer.PreRelease.ToString().Should().Be("beta.25.43");
            semVer.Metadata.Should().BeEmpty();
        }
        [Fact]
        public void Ctor_FileVer_VersionIsParsed()
        {
            //act
            var semVer = new SemVersion(_fileVerString);

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.ToString().Should().BeEmpty();
            semVer.Metadata.Should().BeEmpty();
        }

        [Theory]
        [InlineData("0.0.0", "", "0.0.0")]
        [InlineData("1.2.3", "RC.2", "1.2.3-rc.2")]
        public void Ctor_VersionAndPreReleaseStrings_VersionIsParsed(string version, string prerelease, string semver20String)
        {
            //act
            var semVer = new SemVersion(version, prerelease);

            //assert
            semVer.SemVer20String.Should().Be(semver20String);
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
            semVer.PreRelease.ToString().Should().Be("beta.25.43");
            semVer.Metadata.Should().BeEmpty();
        }

        [Fact]
        public void Ctor_SemVer20WoPreReleaseWithMetadata_VersionIsParsed()
        {
            //act
            var semVer = new SemVersion($"{_fileVerString}+{_metaData}");

            //assert
            semVer.Major.Should().Be(_major);
            semVer.Minor.Should().Be(_minor);
            semVer.Patch.Should().Be(_patch);
            semVer.PreRelease.ToString().Should().BeEmpty();
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
            semVer.PreRelease.ToString().Should().Be(_preRelease);
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
        public void PreRelease_Precedence_PreReleaseIsOnlyConsideredWhenRestIsTheSame()
        {
            var higherWoPreRelease = new SemVersion(1, 0, 1);
            var lowerWithPreRelease = new SemVersion(1, 0, 0, "rc.1");

            //smaller than
            (lowerWithPreRelease < higherWoPreRelease).Should().BeTrue();

            //equals
            (higherWoPreRelease == lowerWithPreRelease).Should().BeFalse();
            (higherWoPreRelease != lowerWithPreRelease).Should().BeTrue();

            //larger than
            (higherWoPreRelease > higherWoPreRelease).Should().BeFalse();
        }

        [Fact]
        public void PreRelease_Precedence_AnyPreReleaseTakesLowerPrecedenceWhenRestIsTheSame()
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
        public void Operators_LargerSmallerThan_VersionsAreComparedCorrectly()
        {
            var lower = new SemVersion("v3.0.0-rc.1");
            var upper = new SemVersion("v3.2.0-beta.1.0");

            (lower < upper).Should().BeTrue();
            (lower > upper).Should().BeFalse();
            (lower == upper).Should().BeFalse();
        }

        [Fact]
        public void MaxMin_Equality_VersionsCanBeOrderedInCollection()
        {
            var min = new SemVersion("v3.0.0-rc.1");
            var middle = new SemVersion("v3.0.0-rc.5");
            var max = new SemVersion("v3.2.0-beta.1.0");

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

        [Fact]// §11 Precedence in https://semver.org/
        public void Precedence_FullExampleFromSemVerDotOrg_VersionsAreInOrder()
        {
            var semVer1 = new SemVersion("1.0.0-alpha");
            var semVer2 = new SemVersion("1.0.0-alpha.1");
            var semVer3 = new SemVersion("1.0.0-alpha.beta");
            var semVer4 = new SemVersion("1.0.0-beta");
            var semVer5 = new SemVersion("1.0.0-beta.2");
            var semVer6 = new SemVersion("1.0.0-beta.11");
            var semVer7 = new SemVersion("1.0.0-rc.1");
            var semVer8 = new SemVersion("1.0.0");


            (semVer1 < semVer2).Should().BeTrue();
            (semVer1 < semVer3).Should().BeTrue();
            (semVer1 < semVer4).Should().BeTrue();
            (semVer1 < semVer5).Should().BeTrue();
            (semVer1 < semVer6).Should().BeTrue();
            (semVer1 < semVer7).Should().BeTrue();
            (semVer1 < semVer8).Should().BeTrue();

            (semVer2 < semVer3).Should().BeTrue();
            (semVer2 < semVer4).Should().BeTrue();
            (semVer2 < semVer5).Should().BeTrue();
            (semVer2 < semVer6).Should().BeTrue();
            (semVer2 < semVer7).Should().BeTrue();
            (semVer2 < semVer8).Should().BeTrue();

            (semVer3 < semVer4).Should().BeTrue();
            (semVer3 < semVer5).Should().BeTrue();
            (semVer3 < semVer6).Should().BeTrue();
            (semVer3 < semVer7).Should().BeTrue();
            (semVer3 < semVer8).Should().BeTrue();

            (semVer4 < semVer5).Should().BeTrue();
            (semVer4 < semVer6).Should().BeTrue();
            (semVer4 < semVer7).Should().BeTrue();
            (semVer4 < semVer8).Should().BeTrue();

            (semVer5 < semVer6).Should().BeTrue();
            (semVer5 < semVer7).Should().BeTrue();
            (semVer5 < semVer8).Should().BeTrue();

            (semVer6 < semVer7).Should().BeTrue();
            (semVer6 < semVer8).Should().BeTrue();

            (semVer7 < semVer8).Should().BeTrue();


            (semVer2 > semVer1).Should().BeTrue();
            (semVer3 > semVer1).Should().BeTrue();
            (semVer4 > semVer1).Should().BeTrue();
            (semVer5 > semVer1).Should().BeTrue();
            (semVer6 > semVer1).Should().BeTrue();
            (semVer7 > semVer1).Should().BeTrue();
            (semVer8 > semVer1).Should().BeTrue();

            (semVer3 > semVer2).Should().BeTrue();
            (semVer4 > semVer2).Should().BeTrue();
            (semVer5 > semVer2).Should().BeTrue();
            (semVer6 > semVer2).Should().BeTrue();
            (semVer7 > semVer2).Should().BeTrue();
            (semVer8 > semVer2).Should().BeTrue();

            (semVer4 > semVer3).Should().BeTrue();
            (semVer5 > semVer3).Should().BeTrue();
            (semVer6 > semVer3).Should().BeTrue();
            (semVer7 > semVer3).Should().BeTrue();
            (semVer8 > semVer3).Should().BeTrue();

            (semVer5 > semVer4).Should().BeTrue();
            (semVer6 > semVer4).Should().BeTrue();
            (semVer7 > semVer4).Should().BeTrue();
            (semVer8 > semVer4).Should().BeTrue();

            (semVer6 > semVer5).Should().BeTrue();
            (semVer7 > semVer5).Should().BeTrue();
            (semVer8 > semVer5).Should().BeTrue();

            (semVer7 > semVer6).Should().BeTrue();
            (semVer8 > semVer6).Should().BeTrue();

            (semVer8 > semVer7).Should().BeTrue();
        }
    }
}

