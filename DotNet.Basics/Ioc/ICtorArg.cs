using System;

namespace DotNet.Basics.Ioc
{
    public interface ICtorArg
    {
        string ParameterName { get; }
        Func<ICsbContainer, object> GetValue { get; }
    }
}
