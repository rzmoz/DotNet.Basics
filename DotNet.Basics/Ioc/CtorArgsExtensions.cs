using System;
using Autofac.Core;

namespace DotNet.Basics.Ioc
{
    internal static class CtorArgsExtensions
    {
        internal static Parameter ToParameter(this ICtorArg arg, IDotNetContainer container)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));
            return new ResolvedParameter((info, context) => info.Name.Equals(arg.ParameterName, StringComparison.InvariantCultureIgnoreCase), (info, context) => arg.GetValue(container));
        }
    }
}
