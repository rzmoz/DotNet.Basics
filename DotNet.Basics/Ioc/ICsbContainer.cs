using System;

namespace DotNet.Basics.Ioc
{
    public interface ICsbContainer : IDisposable
    {
        T Get<T>(IocMode mode = IocMode.Live) where T : class;
        T Get<T>(string bindingName, IocMode mode = IocMode.Live) where T : class;

        object Get(Type type, IocMode mode = IocMode.Live);
        object Get(Type type, string bindingName, IocMode mode = IocMode.Live);

        void BindType<T>(string bindingName = null, IocMode mode = IocMode.Live) where T : class;
        void BindType<T>(Type type, string bindingName = null, IocMode mode = IocMode.Live) where T : class;
        void BindType<T, TK>(string bindingName = null, IocMode mode = IocMode.Live) where TK : class, T where T : class;

        void BindTypeWithCtorArgs<T>(params ICtorArg[] args) where T : class;
        void BindTypeWithCtorArgs<T>(string bindingName = null, IocMode mode = IocMode.Live, params ICtorArg[] args) where T : class;
        void BindTypeWithCtorArgs<T, TK>(params ICtorArg[] args) where T : class where TK : class, T;
        void BindTypeWithCtorArgs<T, TK>(string bindingName = null, IocMode mode = IocMode.Live, params ICtorArg[] args) where T : class where TK : class, T;

        void BindInstance<T>(T instance, string bindingName = null, IocMode mode = IocMode.Live) where T : class;

        void Reset();
    }
}
