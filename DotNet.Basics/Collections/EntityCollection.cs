using System;
using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class EntityCollection : EntityCollection<Entity>
    {
        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(keyLookup, keyNotFound, addKeyExists, orderBy)
        {
        }

        public EntityCollection(IEnumerable<Entity> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(entities, keyLookup, keyNotFound, addKeyExists, orderBy)
        {
        }
    }

    public class EntityCollection<T> : IReadOnlyList<T> where T : Entity
    {
        private readonly Func<IEnumerable<T>, IEnumerable<T>> _orderBy;
        private readonly StringDictionary<T> _dictionary;
        private List<T> _sortedList = new();
        private readonly Action<T> _addAction;

        protected void AddThrowIfKeyExists(T e)
        {
            _dictionary.Add(e.Key, e);
        }
        protected void AddUpdateIfKeyExists(T e)
        {
            _dictionary[e.Key] = e;
        }

        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
            : this(Array.Empty<T>(), keyLookup, keyNotFound, addKeyExists, orderBy)
        { }

        public EntityCollection(IEnumerable<T> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, KeyExists addKeyExists = KeyExists.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
        {
            _orderBy = orderBy ?? GetDefaultSort;
            _dictionary = new StringDictionary<T>(keyLookup, keyNotFound);
            _addAction = addKeyExists == KeyExists.ThrowException ? AddThrowIfKeyExists : AddUpdateIfKeyExists;
            Add(entities.ToArray());
        }
        public EntityCollection<T> Set(params T[] entities)
        {
            foreach (var entity in entities)
                _addAction(entity);
            RefreshSortedList();
            return this;
        }
        public EntityCollection<T> Add(params T[] entities)
        {
            return Set(InitSortOrder(entities, Count));
        }

        private static T[] InitSortOrder(IEnumerable<T> entities, int startIndex)
        {
            var sortOrder = startIndex;
            return entities.Select(e =>
            {
                e.SortOrder = sortOrder++;
                return e;
            }).ToArray();
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

        public virtual int Count => _dictionary.Count;

        public void Sort(Func<IEnumerable<T>, IEnumerable<T>> ordered)
        {
            var sorted = ordered(_dictionary.Values).ToArray();//must flush BEFORE clearing existing items!
            _dictionary.Clear();
            Add(sorted);
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
