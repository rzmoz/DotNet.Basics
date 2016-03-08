using System;

namespace DotNet.Basics.Ioc
{
    public class LazyCtorArg<T> : ICtorArg where T : class
    {
        public LazyCtorArg(CsbContainer container, string parameterName, string getBindingName = null)
        {
            ParameterName = parameterName;
            if (getBindingName == null)
                GetValue = contain => container.Get<T>();
            else
                GetValue = contain => container.Get<T>(getBindingName);
        }

        public string ParameterName { get; }
        public Func<ICsbContainer, object> GetValue { get; }
    }

    public class LazyCtorArg : ICtorArg
    {
        public LazyCtorArg(string parameterName, Func<object> loadValueFunc)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (loadValueFunc == null) throw new ArgumentNullException(nameof(loadValueFunc));
            ParameterName = parameterName;
            GetValue = container => loadValueFunc();
        }




        public string ParameterName { get; }
        public Func<ICsbContainer, object> GetValue { get; }
    }
}
