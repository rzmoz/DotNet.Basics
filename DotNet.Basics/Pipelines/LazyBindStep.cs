using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace DotNet.Basics.Pipelines
{
    public class LazyBindStep<T, TStep> : PipelineStep<T>
        where T : EventArgs, new()
        where TStep : PipelineStep<T>
    {
        public override void Init()
        {
            var step = Container.GetInstance<TStep>();
            DisplayName = step.DisplayName;
        }

        public override async Task RunAsync(T args, ILogger logger)
        {
            var step = Container.GetInstance<TStep>();
            DisplayName = step.DisplayName;
            await step.RunAsync(args, logger).ConfigureAwait(false);
        }
    }
}
