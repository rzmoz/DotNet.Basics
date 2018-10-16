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
            MuteStarted = true;
            MuteEnded = true;
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            var lazyLoadedTask = _loadTask();
            if (lazyLoadedTask == null)
                throw new TaskNotResolvedFromServiceProviderException($"Name: {Name}");

            lazyLoadedTask.EntryLogged += Log.Log;
            await lazyLoadedTask.RunAsync(args, ct).ConfigureAwait(false);
            lazyLoadedTask.EntryLogged -= Log.Log;
        }
    }
}
