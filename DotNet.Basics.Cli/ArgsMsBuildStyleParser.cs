using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;

namespace DotNet.Basics.Cli
{
    public class ArgsMsBuildStyleParser : IArgsParser
    {
        private readonly SysRegex _keyValueStyleRegex = @$"^[{_flagIndicators.Select(c => c.ToString()).JoinString("")}]+(?<key>.+)=(?<value>.+)";
        private static readonly char[] _flagIndicators = ['-', '/'];

        public ArgsDictionary Parse(params IEnumerable<string> args)
        {
            var compiled = new Dictionary<string, List<string>>();

            string? currentKey = null;
            List<string> currentValue = new();

            foreach (var arg in args.Where(a => !string.IsNullOrEmpty(a)))
            {
                if (_keyValueStyleRegex.Test(arg))//is --<key>=<value> style
                {
                    currentKey = null;
                    currentValue = new List<string>();
                    var match = _keyValueStyleRegex.Regex.Match(arg);
                    compiled.Add(TrimKey(match.Groups["key"].Value), match.Groups["value"].Value.Split('|').ToList());
                }
                else if (_flagIndicators.Any(arg.StartsWith))//is flag
                {
                    if (currentKey != null)
                        compiled.Add(currentKey, currentValue);
                    currentKey = TrimKey(arg);
                    currentValue = new List<string>();
                }
                else
                {
                    if (currentKey != null)
                        currentValue.Add(arg);
                }
            }
            if (currentKey != null)
                compiled.Add(currentKey, currentValue);

            return new ArgsDictionary(compiled, TrimKey);
        }

        private string TrimKey(string key)
        {
            return key.TrimStart(_flagIndicators).ToLowerInvariant();
        }
    }
}
