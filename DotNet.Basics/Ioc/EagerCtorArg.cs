using System;

namespace DotNet.Basics.Ioc
{
    public class EagerCtorArg : ICtorArg
    {
        public EagerCtorArg(string parameterName, object value)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (value == null) throw new ArgumentNullException(nameof(value));
            ParameterName = parameterName;
            GetValue = container => value;
        }

        public string ParameterName { get; }
        public Func<IIocContainer, object> GetValue { get; }
    }
}
