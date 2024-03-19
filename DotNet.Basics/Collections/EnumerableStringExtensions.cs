using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Collections
{
    public static class EnumerableStringExtensions
    {
        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, Regex regex)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => regex.IsMatch(element));
        }

        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, string wildCardPattern, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => IsWildcardMatch(element, wildCardPattern, comparison));
        }
        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, IEnumerable<string> whitelist = null, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            whitelist = (whitelist ?? Enumerable.Empty<string>()).ToList();

            foreach (var element in all)
            {
                if (whitelist.Any(incl => element.Equals(incl, comparison)))
                    yield return element;
                else if (whitelist.Any(incl => IsWildcardMatch(element, incl, comparison)))
                    yield return element;
            }
        }

        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, Regex regex)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => !regex.IsMatch(element));
        }

        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, string wildCardPattern, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => !IsWildcardMatch(element, wildCardPattern, comparison));
        }

        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, IEnumerable<string> blacklist = null, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            blacklist = (blacklist ?? Enumerable.Empty<string>()).ToList();

            foreach (var element in all)
            {
                if (blacklist.Any(incl => element.Equals(incl, comparison)))
                    continue;
                if (blacklist.Any(incl => IsWildcardMatch(element, incl, comparison)))
                    continue;
                yield return element;
            }
        }

        private static bool IsWildcardMatch(string input, string needle, StringComparison comparison)
        {
            var pattern = $"^{needle.Replace("*", ".*?")}$";
            var ignoreCase = comparison.ToName().EndsWith("IgnoreCase");
            var regex = ignoreCase ? new Regex(pattern, RegexOptions.IgnoreCase) : new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
}
