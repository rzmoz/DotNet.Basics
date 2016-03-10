using System;

namespace DotNet.Basics.Ioc
{
    public interface IDotNetContainer : IDisposable
    {
        T Get<T>() where T : class;
        T Get<T>(string bindingName) where T : class;

        object Get(Type type);
        object Get(Type type, string bindingName);

        void BindType<T>(string bindingName = null) where T : class;
        void BindType<T>(Type type, string bindingName = null) where T : class;
        void BindType<T, TK>(string bindingName = null) where TK : class, T where T : class;

        void BindTypeWithCtorArgs<T>(params ICtorArg[] args) where T : class;
        void BindTypeWithCtorArgs<T>(string bindingName = null, params ICtorArg[] args) where T : class;
        void BindTypeWithCtorArgs<T, TK>(params ICtorArg[] args) where T : class where TK : class, T;
        void BindTypeWithCtorArgs<T, TK>(string bindingName = null, params ICtorArg[] args) where T : class where TK : class, T;

        void BindInstance<T>(T instance, string bindingName = null) where T : class;

        void Reset();
    }
}
