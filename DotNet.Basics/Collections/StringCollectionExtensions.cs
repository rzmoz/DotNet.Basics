using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Collections
{
    public static class StringCollectionExtensions
    {
        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, IEnumerable<string> whitelist = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            whitelist = (whitelist ?? Enumerable.Empty<string>()).ToList();

            foreach (var element in all)
            {
                if (whitelist.Any(incl => element.Equals(incl, comparison)))
                    yield return element;
                else if (whitelist.Any(incl => IsWildcardMatch(element, incl)))
                    yield return element;
            }
        }

        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, IEnumerable<string> blacklist = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            blacklist = (blacklist ?? Enumerable.Empty<string>()).ToList();

            foreach (var element in all)
            {
                if (blacklist.Any(incl => element.Equals(incl, comparison)))
                    continue;
                if (blacklist.Any(incl => IsWildcardMatch(element, incl)))
                    continue;
                yield return element;
            }
        }

        private static bool IsWildcardMatch(string input, string needle)
        {
            var pattern = $"^{needle.Replace("*", ".*")}$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }
    }
}
