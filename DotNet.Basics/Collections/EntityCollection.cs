using System;
using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class EntityCollection : EntityCollection<Entity>
    {
        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(keyLookup, keyNotFound, orderBy)
        {
        }

        public EntityCollection(IEnumerable<Entity> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, Func<IEnumerable<Entity>, IEnumerable<Entity>> orderBy = null)
            : base(entities, keyLookup, keyNotFound, orderBy)
        {
        }
    }

    public class EntityCollection<T> : IReadOnlyList<T> where T : Entity
    {
        private readonly Func<IEnumerable<T>, IEnumerable<T>> _orderBy;
        private readonly StringDictionary<T> _dictionary;
        private List<T> _sortedList = new();

        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
            : this(Array.Empty<T>(), keyLookup, keyNotFound, orderBy)
        { }

        public EntityCollection(IEnumerable<T> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException, Func<IEnumerable<T>, IEnumerable<T>> orderBy = null)
        {
            _orderBy = orderBy ?? GetDefaultSort;
            _dictionary = new StringDictionary<T>(keyLookup, keyNotFound);
            Add(entities.ToArray());
            RefreshSortedList();
        }

        public EntityCollection<T> Add(params T[] entities)
        {
            return InnerAdd(entities);
        }

        protected virtual EntityCollection<T> InnerAdd(params T[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity.SortOrder == 0)
                    entity.SortOrder = Count + 1;
                _dictionary.Add(entity.Key, entity);
            }
            RefreshSortedList();
            return this;
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
    }
}
