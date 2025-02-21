using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsDefaultParser : IArgsParser
    {
        private static readonly char[] _flagIndicators = ['-', '/'];

        public ArgsDictionary Parse(params IEnumerable<string> args)
        {
            var compiled = new Dictionary<string, string?>();

            string? currentKey = null;
            string? currentValue = null;

            foreach (var arg in args.Where(a => !string.IsNullOrEmpty(a)))
            {


                if (_flagIndicators.Any(arg.StartsWith))//is flag
                {
                    if (currentKey != null)
                        compiled.Add(currentKey, currentValue);
                    currentKey = TrimKey(arg) == string.Empty ? null : TrimKey(arg);
                    currentValue = null;
                }
                else
                {
                    if (currentKey != null)
                        currentValue = arg;
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
