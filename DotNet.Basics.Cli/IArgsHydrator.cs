using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli
{
    public interface IArgsHydrator<T>
    {
        T Hydrate(ICliConfiguration config, T args, ILogDispatcher log = null);
    }
}
