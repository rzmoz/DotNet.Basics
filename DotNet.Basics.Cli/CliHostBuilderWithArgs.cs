using System;
using System.Collections.Generic;


namespace DotNet.Basics.Cli
{
    public class CliHostBuilderWithArgs<T>
    {
        private readonly IList<Func<IArgsHydrator<T>>> _getArgsHydrators = new List<Func<IArgsHydrator<T>>>();

        public CliHostBuilderWithArgs(ICliConfiguration config, bool gydrateArgsFromConfig = true)
        {

            if (gydrateArgsFromConfig)
                WithArgsHydrator(() => new AutoFromConfigHydrator<T>());
        }

        public CliHostBuilderWithArgs<T> WithArgsHydrator(Func<IArgsHydrator<T>> getArgsHydrator)
        {
            if (getArgsHydrator == null) throw new ArgumentNullException(nameof(getArgsHydrator));
            _getArgsHydrators.Add(getArgsHydrator);
            return this;
        }
    }
}
