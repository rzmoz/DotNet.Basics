using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Collections
{
    public class CaseInsensitiveStringDictionary<TK> : IDictionary<string, TK>
    {
        private readonly Dictionary<string, TK> _dictionary = new Dictionary<string, TK>();
        private readonly Func<string, TK> _get;

        public CaseInsensitiveStringDictionary(WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ThrowException)
        {
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

        public TK this[string key]
        {
            get => _get(key);
            set => Add(key, value);
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TK value)
        {
            _dictionary.Add(key.ToLowerInvariant(), value);
        }
        public void Add(KeyValuePair<string, TK> item)
        {
            _dictionary.Add(item.Key.ToLowerInvariant(), item.Value);
        }
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(key.ToLowerInvariant());
        }
        public bool Contains(KeyValuePair<string, TK> item)
        {
            return _dictionary.ContainsKey(item.Key.ToLowerInvariant());
        }

        public void CopyTo(KeyValuePair<string, TK>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.Remove(key.ToLowerInvariant());
        }
        public bool Remove(KeyValuePair<string, TK> item)
        {
            return _dictionary.Remove(item.Key.ToLowerInvariant());
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool TryGetValue(string key, out TK value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return TryGetValue(key.ToLowerInvariant(), out value);
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
