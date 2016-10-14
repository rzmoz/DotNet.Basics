using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNet.Basics.Collections
{
    [Serializable]
    public class StringKeyDictionary<TValue> : IDictionary<string, TValue>, IReadOnlyDictionary<string, TValue>
    {
        private readonly IDictionary<string, TValue> _dic;
        private readonly IDictionary<string, string> _casesInsensitiveMapping;

        public StringKeyDictionary()
            : this(DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode.ThrowKeyNotFoundException)
        {
        }

        public StringKeyDictionary(DictionaryKeyMode dictionaryKeyMode = DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
        {
            KeyNotFoundMode = keyNotFoundMode;
            DictionaryKeyMode = dictionaryKeyMode;
            _dic = new Dictionary<string, TValue>();
            _casesInsensitiveMapping = new Dictionary<string, string>();
        }

        public StringKeyDictionary(IEnumerable<KeyValuePair<string, TValue>> keyValues, DictionaryKeyMode dictionaryKeyMode = DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : this(dictionaryKeyMode, keyNotFoundMode)
        {
            if (keyValues == null)
                return;
            foreach (var keyValue in keyValues)
                Add(keyValue);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return _dic.TryGetValue(ResolvedKey(key), out value);
        }

        public TValue this[string key]
        {
            get { return GetValue(key); }
            set { _dic[ResolvedKey(key)] = value; }
        }

        IEnumerable<string> IReadOnlyDictionary<string, TValue>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<string, TValue>.Values
        {
            get { return Values; }
        }


        public KeyNotFoundMode KeyNotFoundMode { get; }
        public DictionaryKeyMode DictionaryKeyMode { get; }

        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dic.ContainsKey(ResolvedKey(key));
        }

        public void Add(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            Add(new KeyValuePair<string, TValue>(key, value));
        }

        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            try
            {
                return _dic.Remove(ResolvedKey(key));
            }
            finally
            {
                _casesInsensitiveMapping.Remove(key.ToLower());
            }
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            try
            {
                _dic.Add(item.Key, item.Value);
            }
            finally
            {
                _casesInsensitiveMapping.Add(item.Key.ToLower(), item.Key);
            }

        }

        public void Clear()
        {
            try
            {
                _dic.Clear();
            }
            finally
            {
                _casesInsensitiveMapping.Clear();
            }

        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _dic.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            _dic.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dic.Count;
        public bool IsReadOnly => false;

        public ICollection<string> Keys => _dic.Keys;
        public ICollection<TValue> Values => _dic.Values;

        private string ResolvedKey(string key)
        {
            if (DictionaryKeyMode == DictionaryKeyMode.KeyCaseSensitive)
                return key;
            var loweredKey = key.ToLower();
            if (_casesInsensitiveMapping.ContainsKey(loweredKey))
                return _casesInsensitiveMapping[loweredKey];

            if (KeyNotFoundMode == KeyNotFoundMode.ThrowKeyNotFoundException)
                throw new KeyNotFoundException(key);
            return key;
        }

        private TValue GetValue(string key)
        {
            var resolvedKey = ResolvedKey(key);

            TValue outValue;

            if (TryGetValue(resolvedKey, out outValue))
                return outValue;//all is good

            if (KeyNotFoundMode == KeyNotFoundMode.ThrowKeyNotFoundException)
                throw new KeyNotFoundException(key);
            return default(TValue);
        }
    }
}
