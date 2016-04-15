using System;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class LazyBindStep<T, TStep> : PipelineStep<T>
        where T : EventArgs, new()
        where TStep : PipelineStep<T>
    {
        public override void Init()
        {
            var step = Container.Get<TStep>();
            DisplayName = step.DisplayName;
        }

        public override async Task RunAsync(T args, IDiagnostics logger)
        {
            var step = Container.Get<TStep>();
            DisplayName = step.DisplayName;
            await step.RunAsync(args, logger).ConfigureAwait(false);
        }
    }
}
