using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : EventArgs, new()
    {
        private readonly Func<T, Task> _actionAsync;

        public EagerBindStep(Func<T, Task> actionAsync)
        {
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));
            _actionAsync = actionAsync;
        }

        public override async Task RunAsync(T args, CancellationToken ct)
        {
            await _actionAsync.Invoke(args).ConfigureAwait(false);
        }
    }
}
