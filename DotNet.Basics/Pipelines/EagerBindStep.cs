using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : EventArgs, new()
    {
        private readonly Func<T, ILogger, Task> _actionAsync;

        public EagerBindStep(Func<T, ILogger, Task> actionAsync)
        {
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));
            _actionAsync = actionAsync;
        }

        public override async Task RunAsync(T args, ILogger logger)
        {
            await _actionAsync.Invoke(args, logger).ConfigureAwait(false);
        }
    }
}
