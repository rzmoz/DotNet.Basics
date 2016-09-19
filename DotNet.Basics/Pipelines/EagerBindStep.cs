using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : EventArgs, new()
    {
        private readonly Func<T, CancellationToken, Task> _step;

        public EagerBindStep(string name, Func<T, CancellationToken, Task> step) : base(name)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            _step = step;
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            await _step(args, ct).ConfigureAwait(false);
        }
    }
}
