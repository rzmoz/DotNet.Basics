using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class CaseInsensitiveStringDictionaryTests
    {
        [Fact]
        public void Key_CaseInsensitive_ValueIsFound()
        {
            var key = "MyKeYYYYY";
            var value = "sdfsdkguhsdgfkhsdgkshdg";

            var dic = new CaseInsensitiveStringDictionary<string>();
            dic[key] = value;

            var outValueFromLower = dic[key.ToLower()];
            var outValueFromUpper= dic[key.ToUpper()];

            outValueFromLower.Should().Be(value);
            outValueFromUpper.Should().Be(value);
        }

        [Fact]
        public void WhenKeyNotFound_Exception_ExceptionIsThrown()
        {
            var dic = new CaseInsensitiveStringDictionary<string>();
            string temp;
            Action act = () => temp = dic["KeyNotFound"];

            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void WhenKeyNotFound_Default_DefaultIsReturned()
        {
            var dic = new CaseInsensitiveStringDictionary<string>(WhenKeyNotFound.ReturnDefault);

            var temp = dic["KeyNotFound"];

            temp.Should().Be(default(string));
        }
    }
}
