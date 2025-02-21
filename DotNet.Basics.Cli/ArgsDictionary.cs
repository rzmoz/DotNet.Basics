using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsDictionary(IDictionary<string, string?> args, Func<string, string> keyTrimmer) : IReadOnlyDictionary<string, string?>
    {
        public bool Verbose { get; } = HasFlag(nameof(Verbose), keyTrimmer, args);
        public bool ADO { get; } = HasFlag(nameof(ADO), keyTrimmer, args);
        public bool Debug { get; } = HasFlag(nameof(Debug), keyTrimmer, args);

        private readonly IReadOnlyDictionary<string, string?> _args = Compile(args, keyTrimmer);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            throw new NotImplementedException();
        }

        public string? this[string key] => _args[keyTrimmer(key)];
        public IEnumerable<string> Keys => _args.Keys;
        public IEnumerable<string?> Values => _args.Values;
        public bool ContainsKey(string key) => _args.ContainsKey(keyTrimmer(key));
        private static IReadOnlyDictionary<string, string?> Compile(IDictionary<string, string?> args, Func<string, string> keyTrimmer)
        {
            return args
                .Where(a => !IsReservedFlag(a.Key, keyTrimmer))
                .ToDictionary(a => keyTrimmer(a.Key), a => a.Value);
        }
        private static bool IsReservedFlag(string key, Func<string, string> keyTrimmer)
        {
            return keyTrimmer(key).Equals(nameof(Verbose), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(ADO), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(Debug), StringComparison.OrdinalIgnoreCase);
        }
        private static bool HasFlag(string flag, Func<string, string?> keyTrimmer, IDictionary<string, string?> args)
        {
            return args.Any(a => keyTrimmer(a.Key)?.Equals(flag, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
        {
            return _args.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count =>_args.Count;
    }
}
