using System;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class LazyBindStep<T, TStep> : TaskStep<T>
        where T : EventArgs, new()
        where TStep : TaskStep<T>
    {
        public override void Init()
        {
            var step = Container.Get<TStep>(Mode);
            DisplayName = step.DisplayName;
        }

        public override async Task RunAsync(T args, IDiagnostics logger)
        {
            var step = Container.Get<TStep>(Mode);
            DisplayName = step.DisplayName;
            await step.RunAsync(args, logger).ConfigureAwait(false);
        }
    }
}
