using System;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys.Text
{
    public class SysRegex(string pattern, RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline)
    {
        public string Pattern { get; } = pattern;

        public Regex Regex { get; } = new(pattern, options);

        public bool TryMatch(string input, out string match, string returnIfNoMatch = "", int groupNumber = 1)
        {
            var isMatch = Regex.IsMatch(input);
            match = isMatch ? Regex.Match(input).Groups[groupNumber].Value : returnIfNoMatch;
            return isMatch;
        }

        public string Match(string input, string returnIfNoMatch = "", int groupNumber = 1) => Regex.IsMatch(input) ? Regex.Match(input).Groups[groupNumber].Value : returnIfNoMatch;
        public bool IsMatch(string input) => Regex.IsMatch(input);
        public MatchCollection Matches(string input) => Regex.Matches(input);
        public string Replace(string input, string replacement) => Regex.Replace(input, replacement);
        public string Replace(string input, MatchEvaluator evaluator) => Regex.Replace(input, evaluator);
        public string Remove(string input) => Regex.Replace(input, string.Empty);
        
        public static implicit operator SysRegex(string pattern)
        {
            return new SysRegex(pattern ?? throw new ArgumentNullException(nameof(pattern)));
        }

        public override string ToString()
        {
            return Pattern;
        }
    }
}
