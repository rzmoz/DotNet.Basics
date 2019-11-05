using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public static class StringExtensions
    {
        private const string _newlinePattern = @"\r\n|\r|\n";
        private static readonly Regex _newlineRegex = new Regex(_newlinePattern, RegexOptions.Compiled);

        public static string EnsureNewlineHasCarriageReturn(this string source)
        {
            if (source == null)
                return null;
            if (source.Contains('\r'))//if already contains carriage return
                return source;
            if (source.Contains('\n')) //only contains newline but not carriage return
                return source.Replace("\n", "\r\n");

            return source;
        }
        public static string[] ToMultiLine(this string source)
        {
            return source == null ? null : _newlineRegex.Split(source);
        }
        public static string JoinString(this IEnumerable<string> source, string separator = "|")
        {
            return string.Join(separator, source);
        }

        public static string Reverse(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return string.Join("", str.GraphemeClusters().Reverse().ToArray());
        }
        private static IEnumerable<string> GraphemeClusters(this string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }

        public static string ToCamelCase(this string str, CultureInfo cultureInfo = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.ToTitleCase(cultureInfo).Replace(" ", "");
        }

        public static string ToTitleCase(this string str, CultureInfo cultureInfo = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var toUpper = true;
            var output = new StringBuilder(str.Length);
            foreach (var current in str)
            {
                if (toUpper)
                {
                    output.Append(char.ToUpper(current));
                    toUpper = false;
                }
                else
                    output.Append(char.ToLower(current));

                if (current == ' ') //if space
                    toUpper = true;
            }

            return output.ToString();
        }

        public static System.IO.Stream ToStream(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            var stream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string ToMd5(this string text)
        {
            using (var algo = MD5.Create())
            {
                return text.ToHash(algo);
            }
        }
        public static string ToSha1(this string text)
        {
            using (var algo = SHA1.Create())
            {
                return text.ToHash(algo);
            }
        }
        public static string ToSha256(this string text)
        {
            using (var algo = SHA256.Create())
            {
                return text.ToHash(algo);
            }
        }
        public static string ToSha512(this string text)
        {
            using (var algo = SHA512.Create())
            {
                return text.ToHash(algo);
            }
        }

        public static string ToHash(this string text, HashAlgorithm hashAlgorithm)
        {
            return text.ToHash(hashAlgorithm, Encoding.UTF8);
        }

        public static string ToHash(this string text, HashAlgorithm hashAlgorithm, Encoding encoding)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            byte[] bytes = encoding.GetBytes(text);
            byte[] hash = hashAlgorithm.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
        }

        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            var startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex < 0)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }
        public static string EnsurePrefix(this string str, char prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return EnsurePrefix(str, prefix.ToString(), comparison);
        }

        public static string EnsurePrefix(this string str, string prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null || string.IsNullOrEmpty(prefix))
                return str;
            str = str.RemovePrefix(prefix, comparison);
            return prefix + str;
        }

        public static string EnsureSuffix(this string str, char suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return EnsureSuffix(str, suffix.ToString(), comparison);
        }

        public static string EnsureSuffix(this string str, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null || string.IsNullOrEmpty(suffix))
                return str;
            str = str.RemoveSuffix(suffix, comparison);
            return str + suffix;
        }

        public static string RemovePrefix(this string str, char prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return RemovePrefix(str, prefix.ToString(), comparison);
        }

        public static string RemovePrefix(this string str, string prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null || string.IsNullOrEmpty(prefix))
                return str;
            return str.StartsWith(prefix, comparison) ? str.Substring(prefix.Length) : str;
        }

        public static string RemoveSuffix(this string str, char suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return RemoveSuffix(str, suffix.ToString(), comparison);
        }

        public static string RemoveSuffix(this string str, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null || string.IsNullOrEmpty(suffix))
                return str;
            return str.EndsWith(suffix, comparison) ? str.Remove(str.Length - suffix.Length) : str;
        }

        public static string ToBase64(this string str)
        {
            return str.ToBase64(Encoding.UTF8);
        }

        public static string ToBase64(this string str, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            byte[] toEncodeAsBytes = encoding.GetBytes(str);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string FromBase64(this string str)
        {
            return str.FromBase64(Encoding.UTF8);
        }

        public static string FromBase64(this string str, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            byte[] encodedDataAsBytes = Convert.FromBase64String(str);
            return encoding.GetString(encodedDataAsBytes);
        }
    }
}