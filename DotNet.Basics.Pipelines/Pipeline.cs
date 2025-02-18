using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Pipelines
{
    public class Pipeline<T> : ManagedTask<T>
    {
        private readonly Func<IServiceProvider> _getServiceProvider;
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, CancellationToken, Task<int>> _innerRun;

        public Pipeline(string name = null, Invoke invoke = Invoke.Sequential)
            : this(null, name, invoke)
        { }

        public Pipeline(Action<IServiceCollection> configureServices, string name = null, Invoke invoke = Invoke.Sequential)
            : this(GetServiceProvider(configureServices), name, invoke)
        { }

        public Pipeline(Func<IServiceProvider> getServiceProvider, string name = null, Invoke invoke = Invoke.Sequential)
            : base(name, "Pipeline", "Block")
        {
            _getServiceProvider = getServiceProvider ?? new ServiceCollection().BuildServiceProvider;
            _tasks = new ConcurrentQueue<ManagedTask<T>>();
            Invoke = invoke;
            switch (Invoke)
            {
                case Invoke.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case Invoke.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(Invoke)} not supported: {Invoke}");
            }
        }

        public IReadOnlyCollection<ManagedTask<T>> Tasks => _tasks.ToList();
        public Invoke Invoke { get; }

        public bool AssertLazyLoadSteps(ref string errorMessage)
        {
            return AssertLazyLoadSteps(Tasks, ref errorMessage);
        }

        private bool AssertLazyLoadSteps(IReadOnlyCollection<ITask> tasks, ref string errorMessage)
        {
            var success = true;
            foreach (var lazyLoadStep in tasks.OfType<ILazyLoadStep>())
            {
                try
                {
                    lazyLoadStep.GetTask();
                }
                catch (InvalidOperationException e)
                {
                    success = false;
                    errorMessage = $"Failed to load {lazyLoadStep.GetTaskType().Name} - {e.Message}";
                }
            }
            return success;
        }

        public Pipeline<T> AddStep<TTask>(string name = null) where TTask : ManagedTask<T>
        {
            var lazyTask = new LazyLoadStep<T, TTask>(name, _getServiceProvider);
            InitEvents(lazyTask);
            _tasks.Enqueue(lazyTask);
            return this;
        }

        public Pipeline<T> AddStep(Action<T, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task, "Step");
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Action<T, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task, "Step");
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(Func<T, CancellationToken, Task<int>> task)
        {
            var mt = new ManagedTask<T>(task, "Step");
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Func<T, CancellationToken, Task<int>> task)
        {
            var mt = new ManagedTask<T>(name, task, "Step");
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(ManagedTask<T> mt)
        {
            InitEvents(mt);
            _tasks.Enqueue(mt);
            return this;
        }

        public Pipeline<T> AddBlock(params Func<T, CancellationToken, Task<int>>[] tasks)
        {
            return AddBlock(null, tasks);
        }

        public Pipeline<T> AddBlock(string name, params Func<T, CancellationToken, Task<int>>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public Pipeline<T> AddBlock(string name, Invoke invoke = Invoke.Parallel, params Func<T, CancellationToken, Task<int>>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(Pipeline<>));
            var block = new Pipeline<T>(_getServiceProvider, name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddStep(task);
            InitEvents(block);
            _tasks.Enqueue(block);
            return block;
        }

        protected override Task<int> InnerRunAsync(T args, CancellationToken ct)
        {
            return _innerRun(args, ct);
        }

        protected async Task<int> InnerParallelRunAsync(T args, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return 0;
            var tasks = Tasks.Select(task => task.RunAsync(args, ct));
            var results = await Task.WhenAll(tasks);
            return results.Select(i => i < 0 ? i * -1 : i).Sum();
        }

        protected async Task<int> InnerSequentialRunAsync(T args, CancellationToken ct)
        {
            var results = new List<int>();

            foreach (var task in Tasks)
            {
                if (ct.IsCancellationRequested)
                    break;
                results.Add(await task.RunAsync(args, ct));
            }
            return results.Select(i => i < 0 ? i * -1 : i).Sum();
        }

        private void InitEvents(ManagedTask<T> task)
        {
            task.Started += FireStarted;
            task.Ended += FireEnded;
        }

        private static Func<IServiceProvider> GetServiceProvider(Action<IServiceCollection> configuresServices)
        {
            var services = new ServiceCollection();
            configuresServices?.Invoke(services);
            return () => services.BuildServiceProvider();
        }
    }
}
