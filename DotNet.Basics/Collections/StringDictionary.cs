using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary : IReadOnlyCollection<StringPair>
    {
        private readonly KeyNotFoundMode _keyNotFoundMode;
        private readonly StringKeyDictionary<string> _dictionary;

        public StringDictionary()
            : this(KeyMode.CaseSensitive, KeyNotFoundMode.ThrowKeyNotFoundException)
        {
        }

        public StringDictionary(KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : this(Enumerable.Empty<StringPair>(), keyMode, keyNotFoundMode)
        {
        }

        public StringDictionary(IEnumerable<StringPair> keyValues, KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : this(keyValues.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)), keyMode, keyNotFoundMode)
        {
        }

        public StringDictionary(IEnumerable<KeyValuePair<string, string>> keyValues, KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
        {
            _keyNotFoundMode = keyNotFoundMode;
            KeyMode = keyMode;
            _dictionary = new StringKeyDictionary<string>(keyValues, keyMode, keyNotFoundMode);
        }

        public IEnumerator<StringPair> GetEnumerator()
        {
            return _dictionary.Select(kv => new StringPair(kv.Key, kv.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dictionary.Count;

        public bool TryGetValue(string key, out string value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }


        public KeyNotFoundMode KeyNotFoundMode { get; }
        public KeyMode KeyMode { get; }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(string key, string value)
        {
            Add(new StringPair(key, value));
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public void Add(StringPair item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(StringPair item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(StringPair[] array, int arrayIndex)
        {
            _dictionary.Select(kv => new KeyValuePair<string, string>()).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(StringPair item)
        {
            return Remove(item.Key);
        }


        public bool IsReadOnly => false;

        public override string ToString()
        {
            return _dictionary.ToString();
        }

        public ICollection<string> Keys => _dictionary.Keys;
        public ICollection<string> Values => _dictionary.Values;


    }
}
