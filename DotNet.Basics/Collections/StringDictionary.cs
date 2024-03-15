using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary<TK> : IDictionary<string, TK>
    {
        private readonly Dictionary<string, TK> _dictionary;
        private readonly Func<string, TK> _get;
        private readonly Func<string, string> _keyFunc;

        public StringDictionary(WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ThrowException, KeyLookup keyLookup = KeyLookup.IgnoreCase)
        {
            _get = GetGet(whenKeyNotFound);
            _keyFunc = GetKeyFunc(keyLookup);
            _dictionary = new Dictionary<string, TK>();
        }
        public StringDictionary(IDictionary<string, TK> items, WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ThrowException, KeyLookup keyLookup = KeyLookup.IgnoreCase)
        {
            _get = GetGet(whenKeyNotFound);
            _keyFunc = GetKeyFunc(keyLookup);
            _dictionary = items?.ToDictionary(i => _keyFunc(i.Key), i => i.Value) ?? new Dictionary<string, TK>();
        }
        private Func<string, string> GetKeyFunc(KeyLookup keyLookup)
        {
            return keyLookup switch
            {
                KeyLookup.IgnoreCase => key => key.ToLowerInvariant(),
                KeyLookup.CaseSensitive => key => key,
                _ => throw new ArgumentOutOfRangeException(nameof(keyLookup), keyLookup, null)
            };
        }
        private Func<string, TK> GetGet(WhenKeyNotFound whenKeyNotFound)
        {
            return whenKeyNotFound switch
            {
                WhenKeyNotFound.ThrowException => key => _dictionary[key],
                WhenKeyNotFound.ReturnDefault => key => ContainsKey(key) ? _dictionary[key] : default,
                _ => throw new ArgumentException($"{nameof(WhenKeyNotFound)} Not supported: {whenKeyNotFound}")
            };
        }

        public TK this[string key]
        {
            get => _get(_keyFunc(key));
            set => _dictionary[_keyFunc(key)] = value;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TK value)
        {
            _dictionary.Add(_keyFunc(key), value);
        }
        public void Add(KeyValuePair<string, TK> item)
        {
            _dictionary.Add(_keyFunc(item.Key), item.Value);
        }
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(_keyFunc(key));
        }
        public bool Contains(KeyValuePair<string, TK> item)
        {
            return _dictionary.ContainsKey(_keyFunc(item.Key));
        }

        public void CopyTo(KeyValuePair<string, TK>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.Remove(_keyFunc(key));
        }
        public bool Remove(KeyValuePair<string, TK> item)
        {
            return _dictionary.Remove(_keyFunc(item.Key));
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool TryGetValue(string key, out TK value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return TryGetValue(_keyFunc(key), out value);
        }

        public IEnumerator<KeyValuePair<string, TK>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICollection<string> Keys => _dictionary.Keys;
        public ICollection<TK> Values => _dictionary.Values;
    }
}
