using System;

namespace DotNet.Basics.Ioc
{
    public interface ICtorArg
    {
        string ParameterName { get; }
        Func<IIocContainer, object> GetValue { get; }
    }
}
