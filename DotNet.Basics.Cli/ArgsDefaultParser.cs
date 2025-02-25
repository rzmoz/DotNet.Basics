using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsDefaultParser(bool firstEntryIsVerb) : IArgsParser
    {
        private static readonly char[] _flagIndicators = ['-', '/'];

        public ArgsDictionary Parse(params IEnumerable<string> args)
        {
            var compiled = new List<KeyValuePair<string, string?>>();

            string? currentKey = null;
            string? currentValue = null;
            var index = 0;

            foreach (var arg in args.Where(a => !string.IsNullOrEmpty(a)))
            {
                if (firstEntryIsVerb && index == 0)
                {
                    currentKey = TrimKey(arg);
                    currentValue = TrimKey(arg);
                }
                else if (_flagIndicators.Any(arg.StartsWith))//is flag
                {
                    if (currentKey != null)
                        compiled.Add(new KeyValuePair<string, string?>(currentKey, currentValue));
                    currentKey = TrimKey(arg) == string.Empty ? null : TrimKey(arg);
                    currentValue = null;
                }
                else
                {
                    if (currentKey != null)
                        currentValue = arg;
                }

                index++;
            }
            if (currentKey != null)
                compiled.Add(new KeyValuePair<string, string?>(currentKey, currentValue));

            return new ArgsDictionary(compiled, TrimKey, firstEntryIsVerb);
        }

        private string TrimKey(string key)
        {
            return key.TrimStart(_flagIndicators).ToLowerInvariant();
        }
    }
}
