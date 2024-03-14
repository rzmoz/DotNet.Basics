using System;
using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class EntityCollection : EntityCollection<Entity>
    { }

    public class EntityCollection<T> : IEnumerable<T> where T : Entity
    {
        private readonly StringDictionary<T> _entities;
        private readonly Func<string, T> _get;

        public EntityCollection(WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ReturnDefault, KeyLookup keyLookup = KeyLookup.CaseSensitive)
        : this(Array.Empty<T>(), whenKeyNotFound, keyLookup)
        { }

        public EntityCollection(IEnumerable<T> entities, WhenKeyNotFound whenKeyNotFound = WhenKeyNotFound.ReturnDefault, KeyLookup keyLookup = KeyLookup.CaseSensitive)
        {
            _entities = new StringDictionary<T>(entities.ToDictionary(e => e.Key), whenKeyNotFound, keyLookup: keyLookup);
            _get = GetGet(whenKeyNotFound);
        }

        private Func<string, T> GetGet(WhenKeyNotFound whenKeyNotFound)
        {
            return whenKeyNotFound switch
            {
                WhenKeyNotFound.ThrowException => key => _entities[key],
                WhenKeyNotFound.ReturnDefault => key => ContainsKey(key) ? _entities[key] : default,
                _ => throw new ArgumentException($"{nameof(WhenKeyNotFound)} Not supported: {whenKeyNotFound}")
            };
        }

        public EntityCollection<T> Add(params T[] entities)
        {
            foreach (var entity in entities)
                this[entity.Key] = entity;

            return this;
        }

        public T this[string key]
        {
            get => _get(key);
            set => _entities[value.Key] = value;
        }

        public bool ContainsKey(string key)
        {
            return _entities.ContainsKey(key);
        }

        public void Clear()
        {
            _entities.Clear();
        }

        public int Count => _entities.Count;

        public IEnumerator<T> GetEnumerator()
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
