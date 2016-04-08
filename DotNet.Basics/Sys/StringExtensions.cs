using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DotNet.Basics.Sys
{
    public static class StringExtensions
    {
        public static string ToHash(this string text, HashAlgorithm hashAlgorithm)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = hashAlgorithm.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
        }

        public static string ToSha256(this string text)
        {
            using (SHA256 sha256 = new SHA256Managed())
                return text.ToHash(sha256);
        }

        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string EnsurePrefix(this string str, string prefix)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            str = str.RemovePrefix(prefix);
            return prefix + str;
        }

        public static string EnsurePostfix(this string str, string postfix)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (postfix == null) throw new ArgumentNullException(nameof(postfix));

            str = str.RemovePostfix(postfix);
            return str + postfix;
        }

        public static string RemovePrefix(this string str, string prefix)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            if (str.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                return str.Substring(prefix.Length);
            return str;
        }

        public static string RemovePostfix(this string str, string postfix)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (postfix == null) throw new ArgumentNullException(nameof(postfix));

            if (str.EndsWith(postfix, StringComparison.InvariantCultureIgnoreCase))
                return str.Remove(str.Length - postfix.Length);
            return str;
        }

        public static string ToBase64(this string str)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string FromBase64(this string str)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        public static string GZipCompress(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string GZipDecompress(this string compressedString)
        {
            var bytes = Convert.FromBase64String(compressedString);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
