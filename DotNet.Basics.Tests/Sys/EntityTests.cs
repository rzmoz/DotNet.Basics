using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class EntityTests
    {
        [Fact]
        public void CompareTo_IgnoreCase_StringComparisonIsRespected()
        {
            var entLowerCase = new Entity(StringComparison.OrdinalIgnoreCase) { DisplayName = Lorem.Ipsum().ToLowerInvariant() };
            var entUpperCase = new Entity(StringComparison.OrdinalIgnoreCase) { DisplayName = Lorem.Ipsum().ToUpperInvariant() };

            (entLowerCase.DisplayName != entUpperCase.DisplayName).Should().BeTrue();
            entLowerCase.DisplayName.Should().Be(Lorem.Ipsum().ToLowerInvariant());
            entUpperCase.DisplayName.Should().Be(Lorem.Ipsum().ToUpperInvariant());

            entLowerCase.CompareTo(entUpperCase).Should().Be(0);
            entUpperCase.CompareTo(entLowerCase).Should().Be(0);
        }

        [Fact]
        public void Get_OverrideGetters_DescendantPropertyGettersAreInvoked()
        {
            var ent = new LoremIpsumGetterEntity();

            ent.Key.Should().Be(Lorem.Ipsum(2));
            ent.DisplayName.Should().Be(Lorem.Ipsum(4));
        }
        [Fact]
        public void Init_OverrideSetters_DescendantPropertySettersAreInvoked()
        {
            var key = "all_in_lower_case";
            var displayName = "display_name_" + key;
            var ent = new UpperCaseEntity
            {
                Key = key,
                DisplayName = displayName//trigger display name init
            };

            ent.Key.Should().Be(key.ToUpperInvariant());
            ent.DisplayName.Should().Be(displayName.ToUpperInvariant());
        }

        [Fact]
        public void Init_SetFromInit_PropertiesAreSet()
        {
            var key = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();

            var ent = new Entity
            {
                DisplayName = name,
                Key = key
            };

            ent.Key.Should().Be(key);
            ent.DisplayName.Should().Be(name);
        }

        [Fact]
        public void Equals_CompareKeys_KeysAreUsedForEquals()
        {
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();

            var ent1WithKey1 = new Entity { Key = key1 };
            var ent2WithKey1 = new Entity { Key = key1 };
            var ent3WithKey12 = new Entity { Key = key2 };

            ent1WithKey1.Equals(ent2WithKey1).Should().BeTrue();
            ent1WithKey1.Equals(ent3WithKey12).Should().BeFalse();
            ent2WithKey1.Equals(ent3WithKey12).Should().BeFalse();
        }

        [Fact]
        public void CompareTo_FirstSortOrderThenKey_CompareToRespectsSortOrderFirstThenKey()
        {
            var displayNameFirst = "AAA";
            var displayNameLast = "XXX";
            string.Compare(displayNameFirst, displayNameLast, StringComparison.Ordinal).Should().BeLessThan(0);
            var keyFirst = "key1";
            var keyLast = "key2";
            string.Compare(keyFirst, keyLast, StringComparison.Ordinal).Should().BeLessThan(0);

            var ent1 = new Entity { DisplayName = displayNameLast, Key = keyFirst };
            var ent1Duplicate = new Entity { DisplayName = ent1.DisplayName, Key = ent1.Key };
            var ent2 = new Entity { DisplayName = displayNameFirst, Key = keyFirst, SortOrder = -1 };
            var ent3 = new Entity { DisplayName = displayNameFirst, Key = keyFirst };
            var ent4 = new Entity { DisplayName = ent1.DisplayName, Key = keyLast };

            // ReSharper disable once PossibleUnintendedReferenceComparison
            ((ent1 != ent1Duplicate) && ent1.CompareTo(ent1Duplicate) == 0).Should().BeTrue();//same sort order, key and display name but not same object(ref)
            ent2.CompareTo(ent1).Should().BeLessThan(0);//sort order is set - overrides display name
            ent3.CompareTo(ent1).Should().BeLessThan(0);//sort order is not set - display name is used
            ent1.CompareTo(ent4).Should().BeLessThan(0);//sort order and display names are the same. Key is used
        }

        [Fact]
        public void GetHashCode_UseKey_HashcodeIsBasedOnKey()
        {
            var key = Guid.NewGuid().ToString();
            var ent = new Entity { Key = key };
            ent.GetHashCode().Should().Be(key.GetHashCode());
        }

        private class LoremIpsumGetterEntity : Entity
        {
            public override string Key => Lorem.Ipsum(2);
            public override string DisplayName => Lorem.Ipsum(4);
        }

        private class UpperCaseEntity : Entity
        {
            public override string Key => base.Key.ToUpperInvariant();
            public override string DisplayName => base.DisplayName.ToUpperInvariant();
        }
    }
}
