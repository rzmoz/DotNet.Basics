using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class LazyLoadStep<T, TTask> : ManagedTask<T>, ILazyLoadStep
        where T : class, new()
        where TTask : ManagedTask<T>
    {
        private readonly Func<TTask> _getTask;

        public object GetTask()
        {
            return _getTask();
        }

        public Type GetTaskType()
        {
            return typeof(TTask);
        }

        public LazyLoadStep(string name, Func<TTask> getTask) : base(name ?? typeof(TTask).Name)
        {
            _getTask = getTask ?? throw new ArgumentNullException(nameof(getTask));
        }

        protected override async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            var mt = _getTask();
            var result = await mt.RunAsync(args, ct).ConfigureAwait(false);
            issues.AddRange(result.Issues);
        }
    }
}
