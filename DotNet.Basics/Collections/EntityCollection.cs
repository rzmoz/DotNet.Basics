using System;
using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DotNet.Basics.Collections
{
    public class EntityCollection : EntityCollection<Entity>
    {
        [JsonConstructor]
        public EntityCollection() { }
        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(keyLookup, keyNotFound, addKeyExists, orderBy)
        {
        }

        public EntityCollection(IEnumerable<Entity> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(entities, keyLookup, keyNotFound, addKeyExists, orderBy)
        {
        }
    }

    public class EntityCollection<T> : ICollection<T> where T : Entity
    {
        private readonly Func<IEnumerable<T>, IEnumerable<T>> _orderBy;
        private readonly StringDictionary<T> _dictionary;
        private List<T> _sortedList = new();
        private readonly Action<T> _crudAction;

        [JsonConstructor]
        public EntityCollection()
            : this(Array.Empty<T>())
        { }

        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
            : this(Array.Empty<T>(), keyLookup, keyNotFound, addKeyExists, orderBy)
        { }

        public EntityCollection(IEnumerable<T> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
        {
            _orderBy = orderBy ?? GetDefaultSort;
            _dictionary = new StringDictionary<T>(keyLookup, keyNotFound);
            _crudAction = addKeyExists == KeyExists.ThrowException ? e => _dictionary.Add(e.Key, e) : e => _dictionary[e.Key] = e;
            entities.ForEach(Add);
        }
        public EntityCollection<T> Set(params T[] entities)
        {
            foreach (var entity in entities)
                _crudAction(entity);
            RefreshSortedList();
            return this;
        }
        public EntityCollection<T> Add(T[] entities)
        {
            entities.ForEach(Add);
            return this;
        }

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.SortOrder = _dictionary.Count;
            Set(item);
        }

        protected virtual string KeyPreLookup(string key)
        {
            return key;
        }

        public T this[int index] => _sortedList[index];

        public virtual T this[string key]
        {
            get => _dictionary[KeyPreLookup(key)];
            set
            {
                if (_dictionary.TryGetValue(KeyPreLookup(key), out var existing))
                {
                    value.SortOrder = existing.SortOrder;
                    _dictionary[KeyPreLookup(value.Key)] = value;
                    RefreshSortedList();
                }
                else
                    Add(value);
            }
        }

        public virtual bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(KeyPreLookup(key));
        }

        public virtual void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return item != null && _dictionary.ContainsKey(item.Key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var entity in _dictionary.Values)
                array[arrayIndex++] = entity;
        }

        public bool Remove(string key)
        {
            if (key == null)
                return false;
            var removed = _dictionary.Remove(key);
            RefreshSortedList();
            return removed;
        }

        public bool Remove(T item)
        {
            return item != null && Remove(item.Key);
        }

        public virtual int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Sort(Func<IEnumerable<T>, IEnumerable<T>> ordered)
        {
            var sorted = ordered(_dictionary.Values).ToArray();//must flush BEFORE clearing existing items!
            _dictionary.Clear();
            sorted.ForEach(Add);
        }

        protected void RefreshSortedList()
        {
            _sortedList = _orderBy(_dictionary.Values).ToList();
        }

        protected IEnumerable<T> GetDefaultSort(IEnumerable<T> values)
        {
            return values
                .OrderBy(v => v.SortOrder)
                .ThenBy(v => v.DisplayName)
                .ThenBy(v => v.Key);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _sortedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"{Count} : T is {typeof(T)}";
        }
    }
}
