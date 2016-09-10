using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class ConcurrentSet<T> : IReadOnlyCollection<T>
    {
        private readonly ConcurrentDictionary<T, bool> _set;

        public ConcurrentSet()
        {
            _set = new ConcurrentDictionary<T, bool>();
        }

        public ConcurrentSet(IEnumerable<T> collection)
        {
            _set = new ConcurrentDictionary<T, bool>(collection.Select(c => new KeyValuePair<T, bool>(c, false)));
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            _set = new ConcurrentDictionary<T, bool>(comparer);
        }

        public ConcurrentSet(int concurrencyLevel, int capacity)
        {
            _set = new ConcurrentDictionary<T, bool>(concurrencyLevel, capacity);
        }

        public ConcurrentSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _set = new ConcurrentDictionary<T, bool>(collection.Select(c => new KeyValuePair<T, bool>(c, false)), comparer);
        }

        public ConcurrentSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _set = new ConcurrentDictionary<T, bool>(concurrencyLevel, collection.Select(c => new KeyValuePair<T, bool>(c, false)), comparer);
        }

        public ConcurrentSet(int concurrencyLevel, int capacity, IEqualityComparer<T> comparer)
        {
            _set = new ConcurrentDictionary<T, bool>(concurrencyLevel, capacity, comparer);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryAdd(T item)
        {
            return _set.TryAdd(item, false);
        }

        public void Clear()
        {
            _set.Clear();
        }

        public bool Contains(T item)
        {
            return _set.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.Keys.CopyTo(array, arrayIndex);
        }

        public bool TryRemove(T item)
        {
            bool itemOut;
            return _set.TryRemove(item, out itemOut);
        }

        public int Count => _set.Count;
        public bool IsReadOnly => false;
    }
}
