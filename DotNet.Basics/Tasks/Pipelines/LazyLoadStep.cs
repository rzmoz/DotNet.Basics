using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class LazyLoadStep<T, TTask> : ManagedTask<T>, ILazyLoadStep
        where T : class, new()
        where TTask : ManagedTask<T>
    {
        private readonly Func<TTask> _loadTask;

        public object GetTask()
        {
            return _loadTask();
        }

        public Type GetTaskType()
        {
            return typeof(TTask);
        }

        public LazyLoadStep(string name, Func<TTask> loadTask) : base(name ?? typeof(TTask).Name)
        {
            _loadTask = loadTask ?? throw new ArgumentNullException(nameof(loadTask));
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            var mt = _loadTask();
            if (mt == null)
                throw new TaskLoadFailedException($"Name: {Name}");

            mt.EntryLogged += Log;
            await mt.RunAsync(args, ct).ConfigureAwait(false);
            mt.EntryLogged -= Log;
        }
    }
}
