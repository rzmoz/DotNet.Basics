using System;
using System.Globalization;

namespace DotNet.Basics.Sys
{
    public static class StringExtensions
    {
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

            if (cultureInfo == null)
                cultureInfo = CultureInfo.CurrentCulture;

            return cultureInfo.TextInfo.ToTitleCase(str.ToLower(cultureInfo));
        }
    }
}
