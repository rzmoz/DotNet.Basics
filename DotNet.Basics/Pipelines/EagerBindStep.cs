using System;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : EventArgs, new()
    {
        private readonly Func<T, IDiagnostics, Task> _actionAsync;

        public EagerBindStep(Func<T, IDiagnostics, Task> actionAsync)
        {
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));
            _actionAsync = actionAsync;
        }

        public override async Task RunAsync(T args, IDiagnostics logger)
        {
            await _actionAsync.Invoke(args, logger).ConfigureAwait(false);
        }
    }
}
