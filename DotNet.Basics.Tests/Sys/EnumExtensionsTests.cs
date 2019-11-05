using System;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void ToName_ParseRightType_NameIsParsedNotNumValue()
        {
            var result = TestEnum.This.ToName();

            result.Should().Be("This");
        }

        [Fact]
        public void ToEnum_Parse_EnumIsParsed()
        {
            var @enum = "This".ToEnum<TestEnum>();
            @enum.Should().Be(TestEnum.This);
        }

        [Fact]
        public void ToEnum_DefaultValue_EnumIsNotParsed()
        {
            var @enum = "LoremIpsumValueIsNotInEnum".ToEnum<TestEnum>(TestEnum.That);
            @enum.Should().Be(TestEnum.That);
        }

        [Fact]
        public void ToEnum_InvalidInput_ExceptionIsThrown()
        {
            Action act = () => "SomethingNotValidxxxxxxxx".ToEnum<TestEnum>();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ToEnum_ParseCaseInsensitive_EnumIsParsed()
        {
            var @enum = "tHis".ToEnum<TestEnum>();
            @enum.Should().Be(TestEnum.This);
        }

        [Fact]
        public void ToEnum_HasMultiple_EnumHasAll()
        {
            const TestEnum value = TestEnum.This | TestEnum.That;

            value.Has(TestEnum.This).Should().BeTrue();
            value.Has(TestEnum.That).Should().BeTrue();
        }

        [Fact]
        public void IsEnum_Parse_True()
        {
            var result = "This".IsEnum<TestEnum>();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsEnum_InvalidInput_False()
        {
            var result = "SomethingNotValidxxxxxxxx".IsEnum<TestEnum>();
            result.Should().BeFalse();
        }

        [Fact]
        public void GetEnums_Enumerate_EnumsAreGotten()
        {
            var values = typeof(TestEnum).GetEnums<TestEnum>().ToList();
            values.Count.Should().Be(2);
            Action act1 = () => values.Single(v => v == TestEnum.This);
            Action act2 = () => values.Single(v => v == TestEnum.That);

            act1.Should().NotThrow();
            act2.Should().NotThrow();
        }

        private enum TestEnum
        {
            This = 1,
            That = 2
        }
    }
}
