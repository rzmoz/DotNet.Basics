using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Cli
{
    public static class CliHostBuilderExtensions
    {
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            return Build<T>(hostBuilder, () => new T(), null, true, getHydrators);
        }
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, bool autoFillArgs = true, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            return Build<T>(hostBuilder, () => new T(), null, autoFillArgs, getHydrators);
        }
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, Action<ArgsSwitchMappings> customSwitchMappings, bool autoFillArgs = true, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            return Build<T>(hostBuilder, () => new T(), customSwitchMappings, autoFillArgs, getHydrators);
        }
        public static CliHost<T> Build<T>(this CliHostBuilder hostBuilder, Func<T> getArgs, Action<ArgsSwitchMappings> customSwitchMappings, bool autoFillArgs = true, params Func<IArgsHydrator<T>>[] getHydrators) where T : new()
        {
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
            if (getArgs == null) throw new ArgumentNullException(nameof(getArgs));
            var args = getArgs();
            var defaultHost = hostBuilder.Build(customSwitchMappings);

            var hydrators = new List<IArgsHydrator<T>>();
            if (autoFillArgs)
                hydrators.Add(new AutoFromConfigHydrator<T>());//must before custom hydrators
            hydrators.AddRange(getHydrators.Select(get => get.Invoke()));

            hydrators.ForEach(hydrator => hydrator.Hydrate(defaultHost, args, defaultHost.Log));
            return new CliHost<T>(args, defaultHost);
        }
    }
}
