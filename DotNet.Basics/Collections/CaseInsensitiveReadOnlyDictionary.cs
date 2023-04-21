using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class CaseInsensitiveReadOnlyDictionary<TK> : IReadOnlyDictionary<string, TK>
    {
        private readonly ReadOnlyDictionary<string, TK> _dictionary;
        private readonly Func<string, TK> _get;

        public CaseInsensitiveReadOnlyDictionary(IDictionary<string, TK> dictionary, WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ThrowException)
        {
            var formattedDictionary = dictionary.Keys.ToDictionary(dictionaryKey => dictionaryKey.ToLowerInvariant(), dictionaryKey => dictionary[dictionaryKey]);

            _dictionary = new ReadOnlyDictionary<string, TK>(formattedDictionary);

            switch (whenKeyNotFound)
            {
                case WhenKeyNotFound.ThrowException:
                    _get = key =>
                    {
                        if (ContainsKey(key) == false)
                            throw new KeyNotFoundException(key);
                        return _dictionary[key.ToLowerInvariant()];
                    };
                    break;
                case WhenKeyNotFound.ReturnDefault:
                    _get = key => ContainsKey(key) ? _dictionary[key.ToLowerInvariant()] : default(TK);
                    break;
                default:
                    throw new ArgumentException($"{nameof(WhenKeyNotFound)} Not supported: {whenKeyNotFound}");
            }
        }

        public IEnumerator<KeyValuePair<string, TK>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dictionary.Count;
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(key.ToLowerInvariant());
        }

        public bool TryGetValue(string key, out TK value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return TryGetValue(key.ToLowerInvariant(), out value);
        }

        public TK this[string key] => _get(key);

        public IEnumerable<string> Keys => _dictionary.Keys;
        public IEnumerable<TK> Values => _dictionary.Values;
    }
}
