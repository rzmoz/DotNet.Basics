using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class LazyLoadStep<T, TTask> : ManagedTask<T>, ILazyLoadStep
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

        public LazyLoadStep(string name, IServiceProvider serviceProvider) : base(name ?? typeof(TTask).GetNameWithGenericsExpanded(), "Step")
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            _loadTask = () => serviceProvider.GetService(typeof(TTask)) as TTask;
        }

        protected override Task<int> InnerRunAsync(T args)
        {
            var lazyLoadedTask = _loadTask();
            return lazyLoadedTask == null
                ? throw new TaskNotResolvedFromServiceProviderException($"{typeof(TTask).FullName} in {Name}")
                : lazyLoadedTask.RunAsync(args);
        }
    }
}
