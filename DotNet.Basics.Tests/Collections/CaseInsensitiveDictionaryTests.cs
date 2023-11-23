using System;
using System.Collections.Generic;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class CaseInsensitiveDictionaryTests
    {
        [Fact]
        public void Type_MakeReadOnly_IsCaseInsensitiveReadOnly()
        {
            var lst = new List<string>();
        }
        [Fact]
        public void Key_CaseInsensitive_ValueIsFound()
        {
            var key = "MyKeYYYYY";
            var value = "sdfsdkguhsdgfkhsdgkshdg";

            var dic = new CaseInsensitiveDictionary<string>();
            dic[key] = value;

            var outValueFromLower = dic[key.ToLower()];
            var outValueFromUpper= dic[key.ToUpper()];

            outValueFromLower.Should().Be(value);
            outValueFromUpper.Should().Be(value);
        }

        [Fact]
        public void WhenKeyNotFound_Exception_ExceptionIsThrown()
        {
            var dic = new CaseInsensitiveDictionary<string>();
            string temp;
            Action act = () => temp = dic["KeyNotFound"];

            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void WhenKeyNotFound_Default_DefaultIsReturned()
        {
            var dic = new CaseInsensitiveDictionary<string>(WhenKeyNotFound.ReturnDefault);

            var temp = dic["KeyNotFound"];

            temp.Should().Be(default(string));
        }
    }
}
