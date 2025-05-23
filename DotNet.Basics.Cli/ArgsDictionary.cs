﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsDictionary(IReadOnlyList<KeyValuePair<string, string?>> args, Func<string, string> keyTrimmer, bool firstEntryIsVerb) : IReadOnlyDictionary<string, string?>
    {
        public bool FirstEntryIsVerb { get; } = firstEntryIsVerb;
        public bool Verbose { get; } = HasFlag(nameof(Verbose), keyTrimmer, args);
        public bool ADO { get; } = HasFlag(nameof(ADO), keyTrimmer, args);
        public bool Debug { get; } = HasFlag(nameof(Debug), keyTrimmer, args);

        public OrderedDictionary<string, string?> Args { get; } = Compile(args, keyTrimmer);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            return Args.TryGetValue(key, out value);
        }

        public string? this[int index] => Args.GetAt(index).Value;

        public string? this[string key] => Args[keyTrimmer(key)];
        public IEnumerable<string> Keys => Args.Keys;
        public IEnumerable<string?> Values => Args.Values;
        public bool ContainsKey(string key) => Args.ContainsKey(keyTrimmer(key));
        private static OrderedDictionary<string, string?> Compile(IReadOnlyList<KeyValuePair<string, string?>> args, Func<string, string> keyTrimmer)
        {
            return new(args.Where(a => !IsReservedFlag(a.Key, keyTrimmer)));
        }
        private static bool IsReservedFlag(string key, Func<string, string> keyTrimmer)
        {
            return keyTrimmer(key).Equals(nameof(Verbose), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(ADO), StringComparison.OrdinalIgnoreCase) ||
                   keyTrimmer(key).Equals(nameof(Debug), StringComparison.OrdinalIgnoreCase);
        }
        private static bool HasFlag(string flag, Func<string, string?> keyTrimmer, IReadOnlyList<KeyValuePair<string, string?>> args)
        {
            return args.Any(a => keyTrimmer(a.Key)?.Equals(flag, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
        {
            return Args.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Args.Count;
    }
}
