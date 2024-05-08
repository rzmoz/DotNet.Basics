using System;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class DtoTests
    {
        [Fact]
        public void SetDisplayName_WhenKeyIsAlreadySet_KeyIsNotUpdated()
        {
            //arrange
            var displayName = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();


            //act
            var ent = new Dto
            {
                Key = key,
                DisplayName = displayName
            };

            //assert
            ent.DisplayName.Should().Be(displayName);
            ent.Key.Should().Be(key);
        }

        [Fact]
        public void PropertiesUpdated_TriggerEvents_EventsAreTriggered()
        {
            //arrange
            var displayName = Guid.NewGuid().ToString();

            //act
            var ent = new EventDto
            {
                DisplayName = displayName
            };

            //assert
            ent.UpdatedDisplayName.Should().Be(displayName);
            ent.UpdatedKey.Should().Be(displayName);
        }

        [Fact]
        public void Ctor_Serialization_DisplayNameAndKeyAreSerializedCorrectly()
        {
            //arrange
            var ent = new Dto
            {
                DisplayName = "Hello/World!     ",
            };

            //act
            var json = ent.ToJson();
            var deSerEnt = json.FromJson<Dto>();

            //assert
            ent.DisplayName.Should().Be("Hello/World!     ");//display name is kept as is
            ent.Key.Should().Be("hello/world!");//whitespaces are trimmed in keys

            deSerEnt.DisplayName.Should().Be(ent.DisplayName);//display name is kept as is
            deSerEnt.Key.Should().Be("hello/world!");//whitespaces are trimmed in keys
        }

        [Fact]
        public void ImplicitTypeCasting_DisplayName_EntityIsCreatedWithDisplayNameSet()
        {
            var displayName = "Hello World!";

            Dto ent = displayName;

            ent.Key.Should().Be("hello-world!");
            ent.DisplayName.Should().Be(displayName);
        }
        [Fact]
        public void Get_OverrideGetters_DescendantPropertyGettersAreInvoked()
        {
            var ent = new LoremIpsumGetterDto { Key = Lorem.Ipsum(2) };

            ent.Key.Should().Be(Lorem.Ipsum(2));
            ent.DisplayName.Should().Be(Lorem.Ipsum(4));
        }
        [Fact]
        public void Init_OverrideSetters_DescendantPropertySettersAreInvoked()
        {
            var key = "all_in_lower_case";
            var displayName = "display_name_" + key;
            var ent = new UpperCaseDto
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

            var ent = new Dto
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

            var ent1WithKey1 = new Dto { Key = key1 };
            var ent2WithKey1 = new Dto { Key = key1 };
            var ent3WithKey12 = new Dto { Key = key2 };

            ent1WithKey1.Equals(ent2WithKey1).Should().BeTrue();
            ent1WithKey1.Equals(ent3WithKey12).Should().BeFalse();
            ent2WithKey1.Equals(ent3WithKey12).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_UseKey_HashcodeIsBasedOnKey()
        {
            var key = Guid.NewGuid().ToString();
            var ent = new Dto { Key = key };
            ent.GetHashCode().Should().Be(key.GetHashCode());
        }

        private class LoremIpsumGetterDto : Dto
        {
            public override string DisplayName => Lorem.Ipsum(4);
        }

        private class UpperCaseDto : Dto
        {
            public override string DisplayName => base.DisplayName.ToUpperInvariant();
        }
        private class EventDto : Dto
        {
            public string UpdatedDisplayName { get; set; }
            public string UpdatedKey { get; set; }

            protected override void PostDisplayNameSet(string displayName)
            {
                UpdatedDisplayName = displayName;
            }

            protected override void PostKeySet(string key)
            {
                UpdatedKey = key;
            }
        }
    }
}
