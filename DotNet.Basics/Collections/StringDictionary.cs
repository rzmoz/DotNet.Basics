using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary<TK> : IDictionary<string, TK>
    {
        private readonly StringDictionaryOptions<TK> _options;
        private readonly Dictionary<string, TK> _dictionary;

        public StringDictionary(StringDictionaryOptions<TK> options = null)
        {
            _options = options ?? new StringDictionaryOptions<TK>();
            _dictionary = new Dictionary<string, TK>();
        }
        public StringDictionary(IEnumerable<KeyValuePair<string, TK>> items, StringDictionaryOptions<TK> options = null)
        {
            _options = options ?? new StringDictionaryOptions<TK>();
            _dictionary = items?.ToDictionary(i => _options.GetKey(i.Key), i => i.Value) ?? new Dictionary<string, TK>();
        }
        public TK this[string key]
        {
            get => _options.GetValue(key, _dictionary);
            set => _options.SetValue(key, value, _dictionary);
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TK value)
        {
            _dictionary.Add(_options.GetKey(key), value);
        }
        public void Add(KeyValuePair<string, TK> item)
        {
            _dictionary.Add(_options.GetKey(item.Key), item.Value);
        }
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(_options.GetKey(key));
        }
        public bool Contains(KeyValuePair<string, TK> item)
        {
            return _dictionary.ContainsKey(_options.GetKey(item.Key));
        }

        public void CopyTo(KeyValuePair<string, TK>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.Remove(_options.GetKey(key));
        }
        public bool Remove(KeyValuePair<string, TK> item)
        {
            return _dictionary.Remove(_options.GetKey(item.Key));
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool TryGetValue(string key, out TK value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return TryGetValue(_options.GetKey(key), out value);
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
