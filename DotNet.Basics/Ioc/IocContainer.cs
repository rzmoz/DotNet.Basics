using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;

namespace DotNet.Basics.Ioc
{
    public class IocContainer : IIocContainer
    {
        protected IContainer _container;

        public IocContainer(params IIocRegistrations[] registrations)
        {
            _container = null;
            Registrations = new RegistrationsCollection(this);
            Init(registrations);
        }

        public RegistrationsCollection Registrations { get; }

        /// <summary>
        /// Resets all containers to Registrations (which throws away all single modifications later added ea. in tests)
        /// </summary>
        public virtual void Reset()
        {
            Init(Registrations.ToArray());
        }

        private void Init(params IIocRegistrations[] registrations)
        {
            ResetContainers();
            Registrations.Add(registrations);

        }
        private void ResetContainers()
        {
            _container = new ContainerBuilder().Build();
        }

        /**************************************************************************************************************************/
        public virtual T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        public virtual T Get<T>(string bindingName) where T : class
        {
            return (T)Get(typeof(T), bindingName);
        }

        /*--------------------------------------------------------------------------------*/
        public virtual object Get(Type type)
        {
            object instance;
            if (_container.TryResolve(type, out instance))
                return instance;

            //we first see if we have named instances
            var serviceLookup = _container.ComponentRegistry.Registrations.SelectMany(x => x.Services).OfType<KeyedService>().LastOrDefault(x => x.ServiceType == type);

            //we found a named registration and we're just going with that
            if (serviceLookup != null)
            {
                if (_container.TryResolveKeyed(serviceLookup.ServiceKey, serviceLookup.ServiceType, out instance))
                    return instance;
            }

            //there's no registrations whatsoever, so we try to register and resolve it if its something we can resolve
            if (type.IsClass || type.IsEnum || type.IsValueType)
            {
                Bind(builder => builder.RegisterType(type).As(type));
            }
            //last attempt on resolving - if it fails here, we not able to recover. We don't try catch the rest of the code since we optimize for optimistic resolving since that's is the most common scenario in production 
            try
            {
                return _container.Resolve(type);
            }
            catch (ComponentNotRegisteredException e)
            {
                throw new IocException("Ouch", e);
            }
        }

        public virtual object Get(Type type, string bindingName)
        {
            try
            {
                return _container.ResolveNamed(bindingName, type);
            }
            catch (ComponentNotRegisteredException ae)
            {
                throw new IocException(ae.Message);
            }
        }

        /**************************************************************************************************************************/
        public virtual void BindType<T>(string bindingName = null) where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<T>().As<T>());
            else
                Bind(builder => builder.RegisterType<T>().Named<T>(bindingName));
        }

        public virtual void BindType<T>(Type type, string bindingName = null) where T : class
        {
            if (typeof(T).IsAssignableFrom(type) == false)
                throw new ArgumentException($"{type.ToString()} is NOT asignable from {typeof(T).ToString()}");

            if (bindingName == null)
                Bind(builder => builder.RegisterType(type).As<T>().As(type));
            else
                Bind(builder => builder.RegisterType(type).Named<T>(bindingName).Named(bindingName, type));
        }

        public virtual void BindType<T, TK>(string bindingName = null) where TK : class, T where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<TK>().As<T>().As<TK>());
            else
                Bind(builder => builder.RegisterType<TK>().Named<T>(bindingName).Named<TK>(bindingName));
        }

        /**************************************************************************************************************************/

        public virtual void BindTypeWithCtorArgs<T>(params ICtorArg[] args) where T : class
        {
            BindTypeWithCtorArgs<T>(null, args);
        }

        public virtual void BindTypeWithCtorArgs<T>(string bindingName = null, params ICtorArg[] args) where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<T>().As<T>().WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency());
            else
                Bind(builder => builder.RegisterType<T>().Named<T>(bindingName).WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency());
        }

        public virtual void BindTypeWithCtorArgs<T, TK>(params ICtorArg[] args) where T : class where TK : class, T
        {
            BindTypeWithCtorArgs<T, TK>(null, args);
        }

        public virtual void BindTypeWithCtorArgs<T, TK>(string bindingName = null, params ICtorArg[] args) where T : class where TK : class, T
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<TK>().As<T>().As<TK>().WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency());
            else
                Bind(builder => builder.RegisterType<TK>().Named<T>(bindingName).Named<TK>(bindingName).WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency());
        }


        /**************************************************************************************************************************/

        public virtual void BindInstance<T>(T instance, string bindingName = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (bindingName == null)
                Bind(builder => builder.RegisterInstance(instance).As<T>().As(instance.GetType()).SingleInstance());
            else
                Bind(builder => builder.RegisterInstance(instance).Named<T>(bindingName).Named(bindingName, instance.GetType()).SingleInstance());
        }

        /**************************************************************************************************************************/

        protected virtual void Bind(Action<ContainerBuilder> bindAction)
        {
            if (bindAction == null) throw new ArgumentNullException(nameof(bindAction));

            var builder = new ContainerBuilder();
            bindAction.Invoke(builder);
            builder.Update(_container);
        }

        public virtual void Dispose()
        {
            _container.Dispose();
        }

        public IEnumerator<IIocRegistrations> GetEnumerator()
        {
            return Registrations.GetEnumerator();
        }
    }
}
