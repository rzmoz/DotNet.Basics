using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class EagerBindStep<T> : PipelineStep<T> where T : class, new()
    {
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _step;

        public EagerBindStep(string name, Func<T, TaskIssueList, CancellationToken, Task> step) : base(name)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            _step = step;
        }

        protected override async Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            await _step(args, issues, ct).ConfigureAwait(false);
        }
    }
}
