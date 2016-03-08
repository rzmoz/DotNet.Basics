using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;

namespace DotNet.Basics.Ioc
{
    public class CsbContainer : ICsbContainer
    {
        private readonly IocModeCollection<IContainer> _containers;

        public CsbContainer(params ICsbRegistrations[] registrations)
        {
            _containers = new IocModeCollection<IContainer>
            {
                {IocMode.Live, null},
                {IocMode.Debug, null},
                {IocMode.Synthetic, null},
            };
            Registrations = new RegistrationsCollection(this);
            Init(registrations);
        }

        public RegistrationsCollection Registrations { get; }
        protected IContainer this[IocMode mode] => _containers[mode];

        /// <summary>
        /// Resets all containers to Registrations (which throws away all single modifications later added ea. in tests)
        /// </summary>
        public virtual void Reset()
        {
            Init(Registrations.ToArray());
        }

        private void Init(params ICsbRegistrations[] registrations)
        {
            ResetContainers();
            Registrations.Add(registrations);

        }
        private void ResetContainers()
        {
            _containers[IocMode.Live] = new ContainerBuilder().Build();
            _containers[IocMode.Debug] = new ContainerBuilder().Build();
            _containers[IocMode.Synthetic] = new ContainerBuilder().Build();
        }

        /**************************************************************************************************************************/
        public virtual T Get<T>(IocMode mode = IocMode.Live) where T : class
        {
            return (T)Get(typeof(T), mode);
        }

        public virtual T Get<T>(string bindingName, IocMode mode = IocMode.Live) where T : class
        {
            return (T)Get(typeof(T), bindingName, mode);
        }

        /*--------------------------------------------------------------------------------*/
        public virtual object Get(Type type, IocMode mode = IocMode.Live)
        {
            var container = _containers[mode];
            object instance;
            if (container.TryResolve(type, out instance))
                return instance;

            //we first see if we have named instances
            var serviceLookup = container.ComponentRegistry.Registrations.SelectMany(x => x.Services).OfType<KeyedService>().LastOrDefault(x => x.ServiceType == type);

            //we found a named registration and we're just going with that
            if (serviceLookup != null)
            {
                if (container.TryResolveKeyed(serviceLookup.ServiceKey, serviceLookup.ServiceType, out instance))
                    return instance;
            }

            //there's no registrations whatsoever, so we try to register and resolve it if its something we can resolve
            if (type.IsClass || type.IsEnum || type.IsValueType)
            {
                Bind(builder => builder.RegisterType(type).As(type), mode);
                //refresh container
                container = _containers[mode];
            }
            //last attempt on resolving - if it fails here, we not able to recover. We don't try catch the rest of the code since we optimize for optimistic resolving since that's is the most common scenario in production 
            try
            {
                return container.Resolve(type);
            }
            catch (ComponentNotRegisteredException e)
            {
                throw new IocException("Ouch", e);
            }
        }

        public virtual object Get(Type type, string bindingName, IocMode mode = IocMode.Live)
        {
            try
            {
                var container = _containers[mode];
                return container.ResolveNamed(bindingName, type);
            }
            catch (ComponentNotRegisteredException ae)
            {
                throw new IocException(ae.Message);
            }
        }

        /**************************************************************************************************************************/
        public virtual void BindType<T>(string bindingName = null, IocMode mode = IocMode.Live) where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<T>().As<T>(), mode);
            else
                Bind(builder => builder.RegisterType<T>().Named<T>(bindingName), mode);
        }

        public virtual void BindType<T>(Type type, string bindingName = null, IocMode mode = IocMode.Live) where T : class
        {
            if (typeof(T).IsAssignableFrom(type) == false)
                throw new ArgumentException($"{type.ToString()} is NOT asignable from {typeof(T).ToString()}");

            if (bindingName == null)
                Bind(builder => builder.RegisterType(type).As<T>().As(type), mode);
            else
                Bind(builder => builder.RegisterType(type).Named<T>(bindingName).Named(bindingName, type), mode);
        }

        public virtual void BindType<T, TK>(string bindingName = null, IocMode mode = IocMode.Live) where TK : class, T where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<TK>().As<T>().As<TK>(), mode);
            else
                Bind(builder => builder.RegisterType<TK>().Named<T>(bindingName).Named<TK>(bindingName), mode);
        }

        /**************************************************************************************************************************/

        public virtual void BindTypeWithCtorArgs<T>(params ICtorArg[] args) where T : class
        {
            BindTypeWithCtorArgs<T>(null, IocMode.Live, args);
        }

        public virtual void BindTypeWithCtorArgs<T>(string bindingName = null, IocMode mode = IocMode.Live, params ICtorArg[] args) where T : class
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<T>().As<T>().WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency(), mode);
            else
                Bind(builder => builder.RegisterType<T>().Named<T>(bindingName).WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency(), mode);
        }

        public virtual void BindTypeWithCtorArgs<T, TK>(params ICtorArg[] args) where T : class where TK : class, T
        {
            BindTypeWithCtorArgs<T, TK>(null, IocMode.Live, args);
        }

        public virtual void BindTypeWithCtorArgs<T, TK>(string bindingName = null, IocMode mode = IocMode.Live, params ICtorArg[] args) where T : class where TK : class, T
        {
            if (bindingName == null)
                Bind(builder => builder.RegisterType<TK>().As<T>().As<TK>().WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency(), mode);
            else
                Bind(builder => builder.RegisterType<TK>().Named<T>(bindingName).Named<TK>(bindingName).WithParameters(args.Select(arg => arg.ToParameter(this))).InstancePerDependency(), mode);
        }


        /**************************************************************************************************************************/

        public virtual void BindInstance<T>(T instance, string bindingName = null, IocMode mode = IocMode.Live) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (bindingName == null)
                Bind(builder => builder.RegisterInstance(instance).As<T>().As(instance.GetType()).SingleInstance(), mode);
            else
                Bind(builder => builder.RegisterInstance(instance).Named<T>(bindingName).Named(bindingName, instance.GetType()).SingleInstance(), mode);
        }

        /**************************************************************************************************************************/

        protected virtual void Bind(Action<ContainerBuilder> bindAction, IocMode mode)
        {
            if (bindAction == null) throw new ArgumentNullException(nameof(bindAction));

            _containers.Apply(container =>
            {
                var builder = new ContainerBuilder();
                bindAction.Invoke(builder);
                builder.Update(container);
            }, mode);
        }

        public virtual void Dispose()
        {
            Parallel.ForEach(_containers, container => container.Dispose());
        }

        public IEnumerator<ICsbRegistrations> GetEnumerator()
        {
            return Registrations.GetEnumerator();
        }
    }
}
