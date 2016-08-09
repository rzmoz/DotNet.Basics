using System;
using System.Threading.Tasks;
using NLog;

namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : EventArgs, new()
    {
        private readonly Func<T, IPipelineLogger, Task> _actionAsync;

        public EagerBindStep(Func<T, IPipelineLogger, Task> actionAsync)
        {
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));
            _actionAsync = actionAsync;
        }

        public override async Task RunAsync(T args, IPipelineLogger logger)
        {
            await _actionAsync.Invoke(args, logger).ConfigureAwait(false);
        }
    }
}
