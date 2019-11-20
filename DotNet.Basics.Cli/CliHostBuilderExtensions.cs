using System;
using System.Linq;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Cli
{
    public static class CliHostBuilderExtensions
    {
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            return Build<T>(hostBuilder, () => new T(), null, getHydrators);
        }
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, Action<ArgsSwitchMappings> customSwitchMappings, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            return Build<T>(hostBuilder, () => new T(), customSwitchMappings, getHydrators);
        }
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, Func<T> getArgs, Action<ArgsSwitchMappings> customSwitchMappings, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
            if (getArgs == null) throw new ArgumentNullException(nameof(getArgs));
            var args = getArgs();
            var defaultHost = hostBuilder.Build(customSwitchMappings);
            getHydrators
                .Select(get => get.Invoke())
                .ForEach(hydrator => hydrator.Hydrate(defaultHost, args, defaultHost.Log));
            return new CliHost<T>(args, defaultHost);
        }
    }
}
