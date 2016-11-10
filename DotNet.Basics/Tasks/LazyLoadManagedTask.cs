using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class LazyLoadManagedTask<T, TTask> : ManagedTask<T>
        where T : class, new()
        where TTask : ManagedTask<T>
    {
        private readonly Func<TTask> _getTask;

        public LazyLoadManagedTask(string name, Func<TTask> getTask) : base(name ?? typeof(TTask).Name)
        {
            if (getTask == null) throw new ArgumentNullException(nameof(getTask));
            _getTask = getTask;
        }

        protected override async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            var mt = _getTask();
            var result = await mt.RunAsync(args, ct).ConfigureAwait(false);
            issues.Add(result.Issues);
        }
    }
}
