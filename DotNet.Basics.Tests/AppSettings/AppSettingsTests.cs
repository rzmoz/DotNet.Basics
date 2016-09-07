using System;
using DotNet.Basics.AppSettings;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.AppSettings
{
    [TestFixture]
    public class AppSettingsTests
    {
        [Test]
        public void GetValue_RequiredIsSet_ValueIsRetrieved()
        {
            var setting = new AppSetting<string>("RequiredKey");
            setting.GetValue().Should().Be("ValueIsSet");
        }
        [Test]
        public void GetValue_RequiredKeyNotSet_ExceptionIsThrown()
        {
            var key = "MissingKey";
            var setting = new AppSetting<string>(key);
            Action action = () => setting.GetValue();
            action.ShouldThrow<RequiredAppSettingNotFoundException>().WithMessage(key);
        }


        [Test]
        public void GetValue_String_ValueIsRightType()
        {
            AssertValueType<string>("sdfsfsdf");
        }
        [Test]
        public void GetValue_Uri_ValueIsRightType()
        {
            AssertValueType<Uri>(new Uri("http://localhost/"));
        }
        [Test]
        public void GetValue_UShort_ValueIsRightType()
        {
            AssertValueType<ushort>(12312);
        }
        [Test]
        public void GetValue_Short_ValueIsRightType()
        {
            AssertValueType<short>(12312);
        }
        [Test]
        public void GetValue_UInt_ValueIsRightType()
        {
            AssertValueType<uint>(1231232);
        }
        [Test]
        public void GetValue_Int_ValueIsRightType()
        {
            AssertValueType<int>(1231232);
        }
        [Test]
        public void GetValue_Long_ValueIsRightType()
        {
            AssertValueType<long>(DateTime.UtcNow.Ticks);
        }
        [Test]
        public void GetValue_ULong_ValueIsRightType()
        {
            AssertValueType<ulong>(1231232);
        }
        [Test]
        public void GetValue_Double_ValueIsRightType()
        {
            AssertValueType<double>(12.0d);
        }
        [Test]
        public void GetValue_Decimal_ValueIsRightType()
        {
            AssertValueType<decimal>(12.0m);
        }
        [Test]
        public void GetValue_Float_ValueIsRightType()
        {
            AssertValueType<float>(12.0f);
        }
        [Test]
        public void GetValue_Byte_ValueIsRightType()
        {
            AssertValueType<byte>(12);
        }
        [Test]
        public void GetValue_Boolean_ValueIsRightType()
        {
            AssertValueType<bool>(true);
        }

        private void AssertValueType<T>(T value)
        {
            var randomKey = Guid.NewGuid().ToString("N");
            var setting = new AppSetting<T>(randomKey, false, value);
            setting.GetValue().Should().Be(value);
        }
    }
}
