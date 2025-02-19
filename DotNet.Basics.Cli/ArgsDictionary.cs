using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public class ArgsDictionary(ICollection<KeyValuePair<string, List<string>>> args, Func<string, string> keyTrimmer) : IEnumerable<KeyValuePair<string, IReadOnlyList<string>>>
    {
        public bool Verbose { get; } = HasFlag(nameof(Verbose), keyTrimmer, args);
        public bool ADO { get; } = HasFlag(nameof(ADO), keyTrimmer, args);
        public bool Debug { get; } = HasFlag(nameof(Debug), keyTrimmer, args);

        private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _args = Compile(args, keyTrimmer);

        public IReadOnlyList<string> Keys => _args.Keys.ToList();

        public string this[string key] => Get(key).JoinString();
        public IReadOnlyList<string> Get(string key) => _args[keyTrimmer(key)];
        public bool ContainsKey(string key) => _args.ContainsKey(keyTrimmer(key));

        public IEnumerator<KeyValuePair<string, IReadOnlyList<string>>> GetEnumerator()
        {
            return _args.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<string>> Compile(
            IEnumerable<KeyValuePair<string, List<string>>> args, Func<string, string> keyTrimmer)
        {
            return args
                .Where(a => !IsReservedFlag(a.Key, keyTrimmer))
                .Select(a => new KeyValuePair<string, IReadOnlyList<string>>(keyTrimmer(a.Key), a.Value))
                .ToDictionary();
        }
        private static bool IsReservedFlag(string key, Func<string, string> keyTrimmer)
        {
            return keyTrimmer(key).Equals(nameof(Verbose), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(ADO), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(Debug), StringComparison.OrdinalIgnoreCase);
        }
        private static bool HasFlag(string flag, Func<string, string> keyTrimmer, ICollection<KeyValuePair<string, List<string>>> args)
        {
            return args.Any(_ => keyTrimmer(_.Key).Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
