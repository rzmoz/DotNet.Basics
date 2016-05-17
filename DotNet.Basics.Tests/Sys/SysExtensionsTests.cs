using System;
using System.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class SysExtensionsTests
    {
        const string _str = "myStr";
        const string _prefix = "myPrefix";
        const string _postfix = "myPostfix";

        [Test]
        public void ToStream_ConvertToStream_StreamIsGenerated()
        {
            const string myString = "sdfsfsfsdf";
            using (var stream = myString.ToStream())
            {
                var result = new StreamReader(stream).ReadToEnd();
                result.Should().Be(myString);
            }
        }


        [Test]
        public void TryFormat_FormatIsNull_ResultIsStringEmpty()
        {
            string format = null;

            var result = $"{format}";

            result.Should().BeEmpty();
        }

        [Test]
        [TestCase("SHA-256 hash calculator", "409f4b0d60e73274f4704ee0b0f2264b3a9038b63ea13f3eea3dacd9b2f669a7")]
        [TestCase("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "a58dd8680234c1f8cc2ef2b325a43733605a7f16f288e072de8eae81fd8d6433")]
        public void ToSha256_HashString_StringIsHashed(string input, string expectedHash)
        {
            var hashedString = input.ToSha256();
            hashedString.Should().Be(expectedHash);
        }


        [Test]
        [TestCase("xyz", "y", "!", "x!z")]//exclamation mark (special char
        [TestCase("1MyString", "1", "2", "2MyString")]//replacement in start
        [TestCase("MyString1", "1", "2", "MyString2")]//replacement in end
        [TestCase("ooIoo", "i", "X", "ooXoo")]//replacement in middle
        [TestCase("OiOiOiOiOi", "o", "P", "PiPiPiPiPi")]//multiple replacement
        [TestCase("MyString", "mystring", "newValue", "newValue")]//full string replacement
        public void Replace_CaseInsensitive_StringIsReplaces(string orgString, string oldValue, string newValue, string expected)
        {
            var replaced = orgString.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
            replaced.Should().Be(expected);
        }

        [Test]
        public void EnsurePrefix_WhenPrefixDoesntExist_StrIsSame()
        {
            var withPrefix = _str.ToUpper().EnsurePrefix(_prefix);

            withPrefix.Should().Be(_prefix + _str.ToUpper());
        }

        [Test]
        public void EnsurePrefix_WhenPrefixExists_StrIsSame()
        {
            const string str = _prefix + _str;
            var withPrefix = str.EnsurePrefix(_prefix.ToUpper());

            withPrefix.Should().Be(_prefix.ToUpper() + _str);
        }

        [Test]
        public void EnsurePostFix_WhenPostFixDoesntExist_StrIsSame()
        {
            var withPostFix = _str.EnsureSuffix(_postfix);

            withPostFix.Should().Be(_str + _postfix);
        }

        [Test]
        public void EnsurePostFix_WhenPostFixExists_StrIsSame()
        {
            const string str = _str + _postfix;
            var withPostFix = str.EnsureSuffix(_postfix.ToUpper());

            withPostFix.Should().Be(_str + _postfix.ToUpper());
        }

        [Test]
        public void RemovePrefix_WhenPrefixDoesntExist_PrefixRemoved()
        {
            var withoutPrefix = _str.RemovePrefix(_prefix.ToUpper());

            withoutPrefix.Should().Be(_str);
        }

        [Test]
        public void RemovePrefix_WhenPrefixExists_PrefixRemoved()
        {
            const string str = _prefix + _str;
            var withoutPrefix = str.RemovePrefix(_prefix);

            withoutPrefix.Should().Be(_str);
        }


        [Test]
        public void RemovePostfix_WhenPostfixDoesntExist_PostfixRemoved()
        {
            var woPpostFix = _str.RemoveSuffix(_postfix.ToUpper());

            woPpostFix.Should().Be(_str);
        }

        [Test]
        public void RemovePostfix_WhenPostfixExists_PostfixRemoved()
        {
            const string str = _str + _postfix;
            var woPostfix = str.RemoveSuffix(_postfix);

            woPostfix.Should().Be(_str);
        }

        [Test]
        public void ToBase64_Encode_StringIsEncoded()
        {
            "myString".ToBase64().Should().Be("bXlTdHJpbmc=");
        }

        [Test]
        public void FromBase64_Decode_StringIsDecoded()
        {
            "bXlTdHJpbmc=".FromBase64().Should().Be("myString");
        }
    }
}
