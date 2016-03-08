using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotNet.Basics.Ioc
{
    public class IocModeCollection<T> : IEnumerable<T>
    {
        private readonly IDictionary<IocMode, T> _entries;

        public IocModeCollection()
        {
            _entries = new ConcurrentDictionary<IocMode, T>();
        }

        public T this[IocMode mode]
        {
            get
            {
                return _entries[mode];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _entries[mode] = value;
            }
        }

        public void Add(IocMode mode, T entry)
        {
            _entries.Add(mode, entry);
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public void Apply(Action<T> containerModeAction, IocMode mode)
        {
            var container = _entries[mode];
            containerModeAction(container);
            if (mode != IocMode.Live)
                return;
            containerModeAction(_entries[IocMode.Synthetic]);
            containerModeAction(_entries[IocMode.Debug]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
