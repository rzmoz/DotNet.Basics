using System;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class EntityTests
    {
        [Fact]
        public void Ctor_Serialization_DisplayNameAndKeyAreSerializedCorrectly()
        {
            //arrange
            var ent = new Entity
            {
                DisplayName = "Hello World!"
            };

            //act
            var json = ent.ToJson();
            var deSerEnt = json.FromJson<Entity>();

            //assert
            deSerEnt.Key.Should().Be("hello-world");
            deSerEnt.DisplayName.Should().Be(ent.DisplayName);
        }

        [Fact]
        public void ImplicitTypeCasting_DisplayName_EntityIsCreatedWithDisplayNameSet()
        {
            var displayName = "Hello World!";

            Entity ent = displayName;

            ent.Key.Should().Be("hello-world");
            ent.DisplayName.Should().Be(displayName);
        }
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
                DisplayName = displayName,//trigger display name init
                Key = key.ToUpperInvariant(),
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
