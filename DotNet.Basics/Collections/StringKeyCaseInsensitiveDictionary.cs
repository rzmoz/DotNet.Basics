using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    [Serializable]
    public class StringKeyCaseInsensitiveDictionary<TValue> : IDictionary<string, TValue>
    {
        private readonly StringKeyDictionary<KeyValuePair<string, TValue>> _dic;

        public StringKeyCaseInsensitiveDictionary()
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>();
        }

        public StringKeyCaseInsensitiveDictionary(int capacity)
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>(capacity);
        }

        public StringKeyCaseInsensitiveDictionary(IEqualityComparer<string> comparer)
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>(comparer);
        }

        public StringKeyCaseInsensitiveDictionary(IDictionary<string, TValue> dictionary)
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>(dictionary.Count);
            foreach (var value in dictionary)
                _dic.Add(value.Key, value);
        }

        public StringKeyCaseInsensitiveDictionary(int capacity, IEqualityComparer<string> comparer)
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>(capacity, comparer);
        }

        public StringKeyCaseInsensitiveDictionary(IDictionary<string, TValue> dictionary,
            IEqualityComparer<string> comparer)
        {
            _dic = new StringKeyDictionary<KeyValuePair<string, TValue>>(comparer);
            foreach (var value in dictionary)
                _dic.Add(value.Key, value);
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _dic.Values.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count => _dic.Count;
        public bool IsReadOnly => false;
        public bool ContainsKey(string key)
        {
            return _dic.ContainsKey(key.ToLower());
        }

        public void Add(string key, TValue value)
        {
            _dic.Add(key.ToLower(), new KeyValuePair<string, TValue>(key, value));
        }

        public bool Remove(string key)
        {
            return _dic.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out TValue value)
        {
            value = default(TValue);
            KeyValuePair<string, TValue> innerValue;
            var success = _dic.TryGetValue(key.ToLower(), out innerValue);
            if (success)
                value = innerValue.Value;
            return success;
        }

        public TValue this[string key]
        {
            get { return _dic[key.ToLower()].Value; }
            set { _dic[key.ToLower()] = new KeyValuePair<string, TValue>(key, value); }
        }

        public ICollection<string> Keys => _dic.Values.Select(value => value.Key).ToList();
        public ICollection<TValue> Values => _dic.Values.Select(value => value.Value).ToList();
    }
}
