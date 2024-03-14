using DotNet.Basics.Sys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class EntityDictionary : EntityDictionary<Entity>
    {
    }

    public class EntityDictionary<T> : IEnumerable<T> where T : Entity
    {
        private readonly IDictionary<string, T> _entities = new Dictionary<string, T>();

        public EntityDictionary()
        { }

        public EntityDictionary(IEnumerable<T> entities)
        {
            _entities = entities.ToDictionary(e => e.Key);
        }

        public EntityDictionary<T> Add(params T[] entities)
        {
            foreach (var entity in entities)
            {
                this[entity.Key] = entity;
            }
            return this;
        }

        public T this[string key]
        {
            get => _entities[key];
            set => _entities[value.Key] = value;
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
