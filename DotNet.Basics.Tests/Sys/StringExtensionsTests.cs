﻿using System;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class StringExtensionsTests
    {
        const string _str = "myStr";
        const string _prefix = "myPrefix";
        const string _suffix = "myPostfix";

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        [InlineData("lower", "Lower")]
        [InlineData("Upper", "Upper")]
        public void ToUpperFirstChar_Valid_FirstCharIsUpper(string input, string expected)
        {
            var output = input.ToUpperFirstChar();
            output.Should().Be(expected);

            if (!string.IsNullOrWhiteSpace(output))
                char.IsUpper(output[0]).Should().BeTrue();
        }

        [Fact]
        public void EnsureNewlineHasCarriageReturn_NoCarriageReturnNewline_NothingChanges()
        {
            var source = "LoremIpsum";

            var output = source.EnsureNewlineHasCarriageReturn();

            output.Length.Should().Be(source.Length);
            output.Should().Be(source);
        }

        [Fact]
        public void EnsureNewlineHasCarriageReturn_CarriageReturnNewlineAlready_NothingChanges()
        {
            var source = "LoremIpsum\r\nTempo";

            var output = source.EnsureNewlineHasCarriageReturn();

            output.Length.Should().Be(source.Length);
            output.Should().Be(source);
        }

        [Fact]
        public void EnsureNewlineHasCarriageReturn_NewlineButNoCarriageReturn_CarriageReturnFixed()
        {
            var source = "LoremIpsum\nTempo";

            var output = source.EnsureNewlineHasCarriageReturn();

            output.Length.Should().Be(source.Length + 1);
            output.Should().NotBe(source);
        }

        [Fact]
        public void ToMultiLine_Split_StringIsSplintIntoLines()
        {
            var line1 = "lorem";
            var line2 = "ipsum";
            var @string = $"{line1}{Environment.NewLine}{line2}";

            var multiline = @string.ToMultiLine();

            multiline.Length.Should().Be(2);
            multiline.First().Should().Be(line1);
            multiline.Last().Should().Be(line2);
        }

        [Fact]
        public void JoinString_Fluent_MakingItEasierToJoiningStringsSyntax()
        {
            var source = new[] { 0, 1, 2, 3 };

            var pipedString = source.Select(i => i.ToString()).JoinString("|");
            pipedString.Should().Be("0|1|2|3");
            var justString = source.Select(i => i.ToString()).JoinString();
            justString.Should().Be("0123");
        }
        [Theory]
        [InlineData("BobsYourUncle", "elcnUruoYsboB")]
        public void Reverse_ReversString_StringIsReversed(string input, string expected)
        {
            var result = input.Reverse();

            result.Should().Be(expected);
        }

        [Fact]
        public void ToStream_ConvertToStream_StreamIsGenerated()
        {
            const string myString = "sdfsfsfsdf";
            using (var stream = myString.ToStream())
            {
                var result = new System.IO.StreamReader(stream).ReadToEnd();
                result.Should().Be(myString);
            }
        }

        [Fact]
        public void TryFormat_FormatIsNull_ResultIsStringEmpty()
        {
            string format = null;

            var result = $"{format}";

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("MD5 hash calculator", "58347b86ecc13a90ec58f8479394b253")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "35899082e51edf667f14477ac000cbba")]
        public void ToMD51_HashString_StringIsHashed(string input, string expectedHash)
        {
            var hashedString = input.ToMd5();
            hashedString.Should().Be(expectedHash);
        }

        [Theory]
        [InlineData("SHA-1 hash calculator", "1864dd3b5a70a93c46041e73aaa2d40e4de1253c")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "e7505beb754bed863e3885f73e3bb6866bdd7f8c")]
        public void ToSha1_HashString_StringIsHashed(string input, string expectedHash)
        {
            var hashedString = input.ToSha1();
            hashedString.Should().Be(expectedHash);
        }

        [Theory]
        [InlineData("SHA-256 hash calculator", "409f4b0d60e73274f4704ee0b0f2264b3a9038b63ea13f3eea3dacd9b2f669a7")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "a58dd8680234c1f8cc2ef2b325a43733605a7f16f288e072de8eae81fd8d6433")]
        public void ToSha256_HashString_StringIsHashed(string input, string expectedHash)
        {
            var hashedString = input.ToSha256();
            hashedString.Should().Be(expectedHash);
        }

        [Theory]
        [InlineData("SHA-512 hash calculator", "cf504516ca14e41e674c706ea21dc9f6ab0765a50e6404401c59041fbbaf73fce0020375d53271278cd7e71699d3fb0a62878613a7a6f8cb46b7ca1255426968")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "19d8350a48bb40d04b4045955a9d95599aa5bd5b8c74c36c098b58c3cd8af142b8d9cf0417ef6dc88c4ed91c69ea8e2adce7afec1afb6a21d8cae681b0902997")]
        public void ToSha512_HashString_StringIsHashed(string input, string expectedHash)
        {
            var hashedString = input.ToSha512();
            hashedString.Should().Be(expectedHash);
        }

        [Fact]
        public void Replace_ByIndexAndLength_SubstringIsReplaced()
        {
            //arrange
            var input = "Hello World!";

            //act
            var trimmed = input.Replace(4, 5, "!");

            //assert
            trimmed.Should().Be("Hell!ld!");
        }

        [Theory]
        [InlineData("xyz", "y", "!", "x!z")]//exclamation mark (special char
        [InlineData("1MyString", "1", "2", "2MyString")]//replacement in start
        [InlineData("MyString1", "1", "2", "MyString2")]//replacement in end
        [InlineData("ooIoo", "i", "X", "ooXoo")]//replacement in middle
        [InlineData("OiOiOiOiOi", "o", "P", "PiPiPiPiPi")]//multiple replacement
        [InlineData("MyString", "mystring", "newValue", "newValue")]//full string replacement
        public void Replace_CaseInsensitive_StringIsReplaced(string orgString, string oldValue, string newValue, string expected)
        {
            var replaced = orgString.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
            replaced.Should().Be(expected);
        }

        [Theory]
        [InlineData("OiOiOiOiOi", "", new[] { "O", "i" })]//multi remove
        [InlineData("1MyString", "MyString", new[] { "1", "2" })]//misses are ignored
        [InlineData("1MyString", "", new[] { "My", "1String" })]//ordered removal
        [InlineData("1MyString", "1String", "1String", "My")]//ordered removal
        public void Remove_MultipleRemove_EntriesAreRemoved(string orgString, string expected, params string[] removes)
        {
            var replaced = orgString.Remove(removes);
            replaced.Should().Be(expected);
        }

        [Fact]
        public void Remove_SysRegex_RemoveByRegex()
        {
            var orgString = "Hello World!";
            SysRegex regex = @"o";

            //act
            var result = orgString.Remove(regex);

            result.Should().Be("Hell Wrld!");
        }

        [Fact]
        public void EnsurePrefix_WhenPrefixDoesntExist_StrIsSame()
        {
            var withPrefix = _str.ToUpper().EnsurePrefix(_prefix);

            withPrefix.Should().Be(_prefix + _str.ToUpper());
        }

        [Fact]
        public void EnsurePrefix_WhenPrefixExists_StrIsSame()
        {
            const string str = _prefix + _str;
            var withPrefix = str.EnsurePrefix(_prefix.ToUpper());

            withPrefix.Should().Be(_prefix.ToUpper() + _str);
        }

        [Fact]
        public void EnsurePostFix_WhenPostFixDoesntExist_StrIsSame()
        {
            var withPostFix = _str.EnsureSuffix(_suffix);

            withPostFix.Should().Be(_str + _suffix);
        }

        [Fact]
        public void EnsurePostFix_WhenPostFixExists_StrIsSame()
        {
            const string str = _str + _suffix;
            var withPostFix = str.EnsureSuffix(_suffix.ToUpper());

            withPostFix.Should().Be(_str + _suffix.ToUpper());
        }

        [Fact]
        public void RemovePrefix_WhenPrefixDoesntExist_PrefixRemoved()
        {
            var withoutPrefix = _str.RemovePrefix(_prefix.ToUpper());

            withoutPrefix.Should().Be(_str);
        }

        [Fact]
        public void RemovePrefix_WhenPrefixExists_PrefixRemoved()
        {
            const string str = _prefix + _str;
            var withoutPrefix = str.RemovePrefix(_prefix);

            withoutPrefix.Should().Be(_str);
        }


        [Fact]
        public void RemoveSuffix_WhenSuffixDoesntExist_SuffixRemoved()
        {
            var woSuffix = _str.RemoveSuffix(_suffix.ToUpper());

            woSuffix.Should().Be(_str);
        }

        [Fact]
        public void RemoveSuffix_WhenSuffixExists_SuffixRemoved()
        {
            const string str = _str + _suffix;
            var woSuffix = str.RemoveSuffix(_suffix);

            woSuffix.Should().Be(_str);
        }
        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        public void RemoveSuffix_WhenSuffixIsEmpty_SuffixIsRemoved(string suffix)
        {
            var str = "HelloWorld!";
            var woSuffix = str.RemoveSuffix(suffix);

            woSuffix.Should().Be(str);
        }

        [Fact]
        public void ToBase64_Encode_StringIsEncoded()
        {
            "myString".ToBase64().Should().Be("bXlTdHJpbmc=");
        }

        [Fact]
        public void FromBase64_Decode_StringIsDecoded()
        {
            "bXlTdHJpbmc=".FromBase64().Should().Be("myString");
        }

        [Fact]
        public void CamelCaseTest()
        {
            var input = "PoSSeSsIon sO cOMParISon inquietude HE he LOREM";

            var output = input.ToCamelCase();

            output.Should().Be("PossessionSoComparisonInquietudeHeHeLorem");
        }

        [Theory]
        [InlineData("PoSSeSsIon sO cOMParISon inquietude 123 HE1 he LOREM", "Possession So Comparison Inquietude 123 He1 He Lorem")]//normal sentence, random casing
        [InlineData("LoremIpsum", "Lorem Ipsum")]//Camel case
        public void ToTitleCase(string input, string expected)
        {
            var output = input.ToTitleCase();

            output.Should().Be(expected);
        }
    }
}
