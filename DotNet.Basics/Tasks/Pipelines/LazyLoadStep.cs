using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class LazyLoadStep<T, TTask> : ManagedTask<T>
        where T : class, new()
        where TTask : ManagedTask<T>
    {
        public Func<TTask> GetTask { get; }

        public LazyLoadStep(string name, Func<TTask> getTask) : base(name ?? typeof(TTask).Name)
        {
            if (getTask == null) throw new ArgumentNullException(nameof(getTask));
            GetTask = getTask;
        }

        protected override async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            var mt = GetTask();
            var result = await mt.RunAsync(args, ct).ConfigureAwait(false);
            issues.AddRange(result.Issues);
        }
    }
}
