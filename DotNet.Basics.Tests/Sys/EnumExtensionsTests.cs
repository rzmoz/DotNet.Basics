using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class EnumExtensionsTests
    {

        [Test]
        public void IsProperFlagsEnum_Parse_IsGood()
        {
            var result = typeof(TestEnum).IsProperFlagsEnum();

            result.Should().BeTrue();
        }

        [Test]
        public void ToName_ParseRightType_NameIsPArsedNotNumValue()
        {
            var result = TestEnum.This.ToName();

            result.Should().Be("This");
        }

        [Test]
        public void IsProperFlagsEnum_MissingFlagsAttribute_IsGood()
        {
            var result = typeof(EnumWithoutFlagsAttribute).IsProperFlagsEnum();

            result.Should().BeFalse();
        }

        [Test]
        public void IsProperFlagsEnum_ImProperValueSequence_IsGood()
        {
            var result = typeof(EnumWithBadFlagsValueSequence).IsProperFlagsEnum();

            result.Should().BeFalse();
        }

        [Test]
        public void ToEnum_Parse_EnumIsParsed()
        {
            var @enum = "This".ToEnum<TestEnum>();
            @enum.Should().Be(TestEnum.This);
        }

        [Test]
        public void ToEnum_InvalidInput_ExceptionIsThrown()
        {
            Action act = () => "SomethingNotValidxxxxxxxx".ToEnum<TestEnum>();

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void ToEnum_ParseCaseInsensitive_EnumIsParsed()
        {
            var @enum = "tHis".ToEnum<TestEnum>();
            @enum.Should().Be(TestEnum.This);
        }

        [Test]
        public void ToEnum_HasMultiple_EnumHasAll()
        {
            const TestEnum value = TestEnum.This | TestEnum.That;

            value.Has(TestEnum.This).Should().BeTrue();
            value.Has(TestEnum.That).Should().BeTrue();
        }

        [Test]
        public void IsEnum_Parse_True()
        {
            var result = "This".IsEnum<TestEnum>();
            result.Should().BeTrue();
        }

        [Test]
        public void IsEnum_InvalidInput_False()
        {
            var result = "SomethingNotValidxxxxxxxx".IsEnum<TestEnum>();
            result.Should().BeFalse();
        }

        [Test]
        public void FlagsEnums()
        {
            var value = TestEnum.This;
            value.Has(TestEnum.This).Should().BeTrue();

            value = value.Add(TestEnum.That);
            value = value.Remove(TestEnum.This);

            value.Has(TestEnum.This).Should().BeFalse();
            value.Has(TestEnum.That).Should().BeTrue();
        }

        private enum SomeDifferentEnum
        {
            BLaaaaaa
        }

        [Flags]
        private enum TestEnum
        {
            This = 1,
            That = 2,
            More = 4,
            EvenMore = 8,
            EvenMoreMore = 16
        }

        private enum EnumWithoutFlagsAttribute
        {
            This = 1
        }

        [Flags]
        private enum EnumWithBadFlagsValueSequence
        {
            This = 1,
            That = 4,
            Something = 5
        }
    }
}
