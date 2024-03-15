using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary<TK> : IDictionary<string, TK>
    {
        private readonly Dictionary<string, TK> _dictionary;

        private readonly Func<string, string> _getKeyFunc;
        private readonly Func<string, TK> _getValueFunc;

        public StringDictionary(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException)
        : this(Array.Empty<KeyValuePair<string, TK>>(), keyLookup, keyNotFound)
        { }
        public StringDictionary(IEnumerable<KeyValuePair<string, TK>> items, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException)
        {
            _getValueFunc = GetValueFunc(keyNotFound);
            _getKeyFunc = GetKeyFunc(keyLookup);

            _dictionary = items.ToDictionary(i => i.Key, i => i.Value);
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
        private Func<string, TK> GetValueFunc(KeyNotFound keyNotFound)
        {
            return keyNotFound switch
            {
                KeyNotFound.ThrowException => key => _dictionary[key],
                KeyNotFound.ReturnDefault => key => _dictionary.GetValueOrDefault(key),
                _ => throw new ArgumentException($"{nameof(KeyNotFound)} Not supported: {keyNotFound}")
            };
        }

        public virtual TK this[string key]
        {
            get => _getValueFunc(_getKeyFunc(key));
            set => _dictionary[_getKeyFunc(key)] = value;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TK value)
        {
            _dictionary.Add(key, value);
        }
        public void Add(KeyValuePair<string, TK> item)
        {
            _dictionary.Add(_getKeyFunc(item.Key), item.Value);
        }
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(_getKeyFunc(key));
        }
        public bool Contains(KeyValuePair<string, TK> item)
        {
            return _dictionary.ContainsKey(_getKeyFunc(item.Key));
        }

        public void CopyTo(KeyValuePair<string, TK>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.Remove(_getKeyFunc(key));
        }
        public bool Remove(KeyValuePair<string, TK> item)
        {
            return _dictionary.Remove(_getKeyFunc(item.Key));
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool TryGetValue(string key, out TK value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return TryGetValue(_getKeyFunc(key), out value);
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

        public override string ToString()
        {
            return $"{nameof(Count)}: {Count}";
        }
    }
}
