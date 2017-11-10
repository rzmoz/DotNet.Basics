using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void ToName_ParseRightType_NameIsPArsedNotNumValue()
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
        public void ToEnum_InvalidInput_ExceptionIsThrown()
        {
            Action act = () => "SomethingNotValidxxxxxxxx".ToEnum<TestEnum>();

            act.ShouldThrow<ArgumentException>();
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
        

        private enum TestEnum
        {
            This = 1,
            That = 2,
            More = 4,
            EvenMore = 8,
            EvenMoreMore = 16
        }
    }
}
