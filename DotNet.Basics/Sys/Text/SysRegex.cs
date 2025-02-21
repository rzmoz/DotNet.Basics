using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys.Text
{
    public class SysRegex(string pattern, RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline)
    {
        public string Pattern { get; } = pattern;

        public Regex Regex { get; } = new(pattern, options);

        public bool TryMatch(string input, out string match, string returnIfNoMatch = "", int groupNumber = 1)
        {
            var isMatch = Regex.IsMatch(input);
            match = isMatch ? GetGroup(Regex.Match(input).Groups, groupNumber).Value : returnIfNoMatch;
            return isMatch;
        }

        public string Match(string input, string returnIfNoMatch = "", int groupNumber = 1)
        {
            TryMatch(input, out var match, returnIfNoMatch, groupNumber);
            return match;
        }

        public bool Test(string input) => Regex.IsMatch(input);
        public MatchCollection Matches(string input) => Regex.Matches(input);
        public string Replace(string input, string replacement) => Regex.Replace(input, replacement);
        public string Replace(string input, MatchEvaluator evaluator) => Regex.Replace(input, evaluator);
        public string Remove(string input) => Regex.Replace(input, string.Empty);

        private Group GetGroup(GroupCollection groups, int groupNumber)
        {
            return groups.Count > groupNumber ? groups[groupNumber] : groups.First();
        }

        public static implicit operator SysRegex(string pattern)
        {
            return new SysRegex(pattern ?? throw new ArgumentNullException(nameof(pattern)));
        }

        public static implicit operator string(SysRegex r)
        {
            return r.Pattern;
        }

        public override string ToString()
        {
            return Pattern;
        }
    }
}
