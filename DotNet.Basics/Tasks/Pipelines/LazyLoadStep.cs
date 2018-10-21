using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

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

        public LazyLoadStep(string name, Func<IServiceProvider> getServiceProviderTask) : base(name ?? typeof(TTask).GetNameWithGenericsExpanded())
        {
            if (getServiceProviderTask == null) throw new ArgumentNullException(nameof(getServiceProviderTask));

            _loadTask = () =>
            {
                var serviceProvider = getServiceProviderTask.Invoke();
                if (serviceProvider == null)
                    throw new ServiceProviderIsNullException(Name);
                return serviceProvider.GetService(typeof(TTask)) as TTask;
            };
        }

        protected override Task InnerRunAsync(T args, CancellationToken ct)
        {
            var lazyLoadedTask = _loadTask();
            if (lazyLoadedTask == null)
                throw new TaskNotResolvedFromServiceProviderException(Name);

            try
            {
                lazyLoadedTask.EntryLogged += Log.Log;
                return lazyLoadedTask.RunAsync(args, ct);
            }
            finally
            {
                lazyLoadedTask.EntryLogged -= Log.Log;
            }
        }
    }
}
