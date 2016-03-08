using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Ioc
{
    public class RegistrationsCollection : IReadOnlyCollection<ICsbRegistrations>
    {
        private readonly ICsbContainer _container;
        private readonly IList<RegistrationsCollectionItem> _registrations;

        public RegistrationsCollection(ICsbContainer container
            )
        {
            _container = container;
            _registrations = new List<RegistrationsCollectionItem>();
        }

        public void Add<T>() where T : ICsbRegistrations, new()
        {
            Add(new T());
        }

        public void Add(ICsbRegistrations registrations)
        {
            Add(new[] { registrations });
        }

        public void Add(params ICsbRegistrations[] registrations)
        {
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));

            foreach (var registration in registrations)
            {
                var item = new RegistrationsCollectionItem(registration);
                if (_registrations.Contains(item))
                    _registrations[_registrations.IndexOf(item)] = item;
                else
                    _registrations.Add(item);
            }

            foreach (var registration in registrations)
            {
                registration.RegisterIn(_container);
            }
        }

        public void Clear()
        {
            _registrations.Clear();
        }

        public IEnumerator<ICsbRegistrations> GetEnumerator()
        {
            return _registrations.Select(regItem => regItem.Registration).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _registrations.Count;
    }
}
