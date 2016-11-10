using System;
using System.Diagnostics;
using System.Linq;
using DotNet.Basics.AppSettings;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;


namespace DotNet.Basics.Tests.AppSettings
{

    public class AppSettingsTests
    {
        [Fact]
        public void Ctor_CustomParserArray_ValueIsParsed()
        {
            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.Get(Arg.Any<string>()).Returns("['1','2,','3']");

            var setting = new AppSetting<string[]>("Ctor_CustomParserArray_ValueIsParsed",
                JsonConvert.DeserializeObject<string[]>, configurationManager);

            var parsed = setting.GetValue();

            parsed.Length.Should().Be(3);
            parsed.First().Should().Be("1");
            parsed.Last().Should().Be("3");
        }

        [Fact]
        public void GetValue_FromSystemConfigurationManager_ValueIsRetrieved()
        {
            var setting = new AppSetting("RequiredKey");
            setting.GetValue().Should().Be("ValueIsSet");
        }
        [Fact]
        public void GetValue_RequiredKeyNotSet_ExceptionIsThrown()
        {
            var key = "MissingKey";
            var setting = new AppSetting<string>(key);
            Action action = () => setting.GetValue();
            action.ShouldThrow<RequiredConfigurationKeyNotSetException>().And.MissingKeys.Single().Should().Be(key);
        }

        /************** DotNet.Basics Types **************/
        [Theory]
        [InlineData(@"c:\mypath\")]//absolute path
        [InlineData(@"\mypath\")]//relative path
        public void GetValue_DirPath_ValueIsRightType(string path)
        {
            AssertValueType<DirPath>(path.ToDir());
        }

        [Theory]
        [InlineData(@"c:\mypath\myfile.txt")]//absolute path
        [InlineData(@"\mypath\myfile.txt")]//relative path
        public void GetValue_FilePath_ValueIsRightType(string path)
        {
            AssertValueType<FilePath>(path.ToFile());
        }

        /************** Non-Simple System Types **************/

        [Fact]
        public void GetValue_String_ValueIsRightType()
        {
            AssertValueType<string>("sdfsfsdf");
        }

        [Fact]
        public void GetValue_Uri_ValueIsRightType()
        {
            AssertValueType<Uri>("http://localhost/", new Uri("http://localhost/"));
        }

        //https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx
        [Theory]
        [InlineData("")]
        [InlineData("N")]
        [InlineData("D")]
        [InlineData("B")]
        [InlineData("P")]
        [InlineData("X")]
        public void GetValue_Guid_ValueIsRightType(string toStringFormatter)
        {
            var guid = Guid.NewGuid();
            AssertValueType<Guid>(guid.ToString(toStringFormatter), guid);
        }

        /************** Integral Types **************/

        [Fact]
        public void GetValue_SByte_ValueIsRightType()
        {
            AssertValueType<sbyte>(11);
        }
        [Fact]
        public void GetValue_Byte_ValueIsRightType()
        {
            AssertValueType<byte>(12);
        }

        [Fact]
        public void GetValue_Short_ValueIsRightType()
        {
            AssertValueType<short>(12312);
        }
        [Fact]
        public void GetValue_UShort_ValueIsRightType()
        {
            AssertValueType<ushort>(12312);
        }

        [Fact]
        public void GetValue_Int_ValueIsRightType()
        {
            AssertValueType<int>(1231232);
        }
        [Fact]
        public void GetValue_UInt_ValueIsRightType()
        {
            AssertValueType<uint>(1231232);
        }

        [Fact]
        public void GetValue_Long_ValueIsRightType()
        {
            AssertValueType<long>(DateTime.UtcNow.Ticks);
        }
        [Fact]
        public void GetValue_ULong_ValueIsRightType()
        {
            AssertValueType<ulong>((ulong)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void GetValue_Char_ValueIsRightType()
        {
            AssertValueType<char>('@');
        }

        /************** Floating Point Types **************/

        [Fact]
        public void GetValue_Float_ValueIsRightType()
        {
            AssertValueType<float>("12.0", 12.0f);
        }

        [Fact]
        public void GetValue_Double_ValueIsRightType()
        {
            AssertValueType<double>("12.0", 12.0d);
        }

        /************** The Decimal Type **************/

        [Fact]
        public void GetValue_Decimal_ValueIsRightType()
        {
            AssertValueType<decimal>("12.0", 12.0m);
        }

        /************** The Bool Type **************/

        [Fact]
        public void GetValue_Boolean_ValueIsRightType()
        {
            AssertValueType<bool>("TrUe", true);
            AssertValueType<bool>("true", true);
            AssertValueType<bool>("TRUE", true);
        }

        /************** Assertions **************/

        private void AssertValueType<T>(T expected)
        {
            AssertValueType<T>(expected.ToString(), expected);
        }
        private void AssertValueType<T>(string input, T expected)
        {
            Debug.WriteLine($"Asserting appsetting: {input} conversion to {typeof(T).FullName}");

            var randomKey = Guid.NewGuid().ToString("N");
            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.Get(Arg.Any<string>()).Returns(input);
            var setting = new AppSetting<T>(randomKey, configurationManager: configurationManager);
            setting.GetValue().Should().Be(expected);
        }
    }
}
