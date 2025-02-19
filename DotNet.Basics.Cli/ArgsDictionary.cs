using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public class ArgsDictionary(IEnumerable<KeyValuePair<string, List<string>>> args, Func<string, string> keyTrimmer)
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _args = args.Select(a => new KeyValuePair<string, IReadOnlyList<string>>(a.Key.TrimStart('-').ToLowerInvariant(), a.Value)).ToDictionary();

        public string this[string key] => Get(key).JoinString();
        public IReadOnlyList<string> Get(string key) => _args[keyTrimmer(key)];
        public bool ContainsKey(string key) => _args.ContainsKey(keyTrimmer(key));
    }
}
