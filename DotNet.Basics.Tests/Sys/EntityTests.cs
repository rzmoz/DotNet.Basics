﻿using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class EntityTests
    {
        [Fact]
        public void Get_OverrideGetters_DescendantPropertyGettersAreInvoked()
        {
            var ent = new LoremIpsumGetterEntity { Key = Lorem.Ipsum(2) };

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
                Key = key.ToUpperInvariant(),
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
                Key = key,
                DisplayName = name,
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
        public void GetHashCode_UseKey_HashcodeIsBasedOnKey()
        {
            var key = Guid.NewGuid().ToString();
            var ent = new Entity { Key = key };
            ent.GetHashCode().Should().Be(key.GetHashCode());
        }

        private class LoremIpsumGetterEntity : Entity
        {
            public override string DisplayName => Lorem.Ipsum(4);
        }

        private class UpperCaseEntity : Entity
        {
            public override string DisplayName => base.DisplayName.ToUpperInvariant();
        }
    }
}
