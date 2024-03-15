using System;
using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class EntityCollection : EntityCollection<Entity>
    { }

    public class EntityCollection<T> : IReadOnlyCollection<T> where T : Entity
    {
        private readonly StringDictionary<T> _entities;

        public EntityCollection(KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException)
            : this(Array.Empty<T>(), keyLookup, keyNotFound)
        { }

        public EntityCollection(IEnumerable<T> entities, KeyLookup keyLookup = KeyLookup.CaseSensitive, KeyNotFound keyNotFound = KeyNotFound.ThrowException)
        {
            _entities = new StringDictionary<T>(entities.ToDictionary(e => e.GetKey()), keyLookup, keyNotFound);
        }

        public virtual EntityCollection<T> Add(params T[] entities)
        {
            foreach (var entity in entities)
                this[entity.GetKey()] = entity;
            return this;
        }

        protected virtual string KeyPreLookup(string key)
        {
            return key;
        }

        public virtual T this[string key]
        {
            get => _entities[KeyPreLookup(key)];
            set => _entities[KeyPreLookup(value.GetKey())] = value;
        }

        public virtual bool ContainsKey(string key)
        {
            return _entities.ContainsKey(KeyPreLookup(key));
        }

        public virtual void Clear()
        {
            _entities.Clear();
        }

        public virtual int Count => _entities.Count;

        public virtual IEnumerator<T> GetEnumerator()
        {
            var sortedList = _entities.Values.ToList();
            sortedList.Sort();
            return sortedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
