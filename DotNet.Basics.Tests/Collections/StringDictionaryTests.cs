using System;
using System.Collections.Generic;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class StringDictionaryTests
    {
        [Fact]
        public void Key_CaseInsensitive_ValueIsFound()
        {
            var key = "MyKeYYYYY";
            var value = "sdfsdkguhsdgfkhsdgkshdg";

            var dic = new StringDictionary<string>(keyLookup: KeyLookup.IgnoreCase)
            {
                [key] = value
            };

            var outValueFromLower = dic[key.ToLower()];
            var outValueFromUpper = dic[key.ToUpper()];

            outValueFromLower.Should().Be(value);
            outValueFromUpper.Should().Be(value);
        }
        [Fact]
        public void Key_CaseSensitive_ValueIsNotFound()
        {
            var key = "MyKeYYYYY";
            
            var dic = new StringDictionary<string>(keyLookup: KeyLookup.CaseSensitive)
            {
                [key] = Lorem.Ipsum(5000)
            };
            string temp;
            Action act1 = () => temp = dic[key.ToLower()];
            Action act2 = () => temp = dic[key.ToUpper()];

            act1.Should().Throw<KeyNotFoundException>();
            act2.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void WhenKeyNotFound_Exception_ExceptionIsThrown()
        {
            var dic = new StringDictionary<string>();
            string temp;
            Action act = () => temp = dic["KeyNotFound"];

            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void WhenKeyNotFound_Default_DefaultIsReturned()
        {
            var dic = new StringDictionary<string>(whenKeyNotFound: WhenKeyNotFound.ReturnDefault);

            var temp = dic["KeyNotFound"];

            temp.Should().Be(default);
        }
    }
}
