using System;
using System.Reflection;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class LazyLoadStep<T, TTask> : ManagedTask<T>, ILazyLoadStep
        where TTask : ManagedTask<T>
    {
        private readonly Func<TTask?> _loadTask;

        public object GetTask()
        {
            return _loadTask() ?? throw new TaskNotResolvedFromServiceProviderException($"{typeof(TTask).FullName} in {Name}");
        }

        public Type GetTaskType()
        {
            return typeof(TTask);
        }

        public LazyLoadStep(string? name, IServiceProvider serviceProvider) : base(name ?? typeof(TTask).GetNameWithGenericsExpanded())
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            _loadTask = () =>
            {
                var task = serviceProvider.GetService(typeof(TTask)) as TTask;
                if (task == null)
                    return task;
                var nameProp = typeof(ManagedTask).GetProperty(nameof(Name), BindingFlags.Public | BindingFlags.Instance);
                var nameSetter = nameProp.DeclaringType.GetField($"<{nameof(Name)}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                nameSetter.SetValue(task, Name);
                return task;
            };
        }

        protected override async Task<int> InnerRunAsync(T args)
        {
            var lazyLoadedTask = _loadTask();
            return lazyLoadedTask == null
                ? throw new TaskNotResolvedFromServiceProviderException($"{typeof(TTask).FullName} in {Name}")
                : await lazyLoadedTask.RunAsync(args);
        }
    }
}
