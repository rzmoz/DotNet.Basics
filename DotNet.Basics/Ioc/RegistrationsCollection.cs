using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Ioc
{
    public class RegistrationsCollection : IReadOnlyCollection<IDotNetRegistrations>
    {
        private readonly IIocContainer _container;
        private readonly IList<RegistrationsCollectionItem> _registrations;

        public RegistrationsCollection(IIocContainer container
            )
        {
            _container = container;
            _registrations = new List<RegistrationsCollectionItem>();
        }

        public void Add<T>() where T : IDotNetRegistrations, new()
        {
            Add(new T());
        }

        public void Add(IDotNetRegistrations registrations)
        {
            Add(new[] { registrations });
        }

        public void Add(params IDotNetRegistrations[] registrations)
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

        public IEnumerator<IDotNetRegistrations> GetEnumerator()
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
