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

        public EntityCollection(IEnumerable<T> entities = null, StringDictionaryOptions<T> options = null)
        {
            _entities = new StringDictionary<T>(entities?.ToDictionary(e => e.Key) ?? new Dictionary<string, T>(), options ?? new StringDictionaryOptions<T>
            {
                KeyNotFound = KeyNotFound.ThrowException,
                KeyLookup = KeyLookup.IgnoreCase
            });
        }

        public virtual EntityCollection<T> Add(params T[] entities)
        {
            foreach (var entity in entities)
                this[entity.Key] = entity;

            return this;
        }

        public virtual T this[string key]
        {
            get => _entities[key];
            set => _entities[value.Key] = value;
        }

        public virtual bool ContainsKey(string key)
        {
            return _entities.ContainsKey(key);
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
