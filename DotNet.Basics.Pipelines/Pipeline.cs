using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Pipelines
{
    public class Pipeline<T> : ManagedTask<T>
    {
        private readonly Func<IServiceProvider> _getServiceProvider;
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, LogDispatcher, CancellationToken, Task> _innerRun;

        public Pipeline(string name = null, Invoke invoke = Invoke.Sequential)
            : this(null, name, invoke)
        { }

        public Pipeline(Action<IServiceCollection> configureServices, string name = null, Invoke invoke = Invoke.Sequential)
            : this(GetServiceProvider(configureServices), name, invoke)
        { }

        public Pipeline(Func<IServiceProvider> getServiceProvider, string name = null, Invoke invoke = Invoke.Sequential)
            : base(name)
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
                    throw new ArgumentException($"{nameof(Invoke)} not supported: {Invoke }");
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

        public Pipeline<T> AddStep(Action<T, LogDispatcher, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Action<T, LogDispatcher, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(Func<T, LogDispatcher, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Func<T, LogDispatcher, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(ManagedTask<T> mt)
        {
            InitEvents(mt);
            _tasks.Enqueue(mt);
            return this;
        }

        public Pipeline<T> AddBlock(params Func<T, LogDispatcher, CancellationToken, Task>[] tasks)
        {
            return AddBlock(null, tasks);
        }

        public Pipeline<T> AddBlock(string name, params Func<T, LogDispatcher, CancellationToken, Task>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public Pipeline<T> AddBlock(string name, Invoke invoke = Invoke.Parallel, params Func<T, LogDispatcher, CancellationToken, Task>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(Pipeline<>));
            var block = new Pipeline<T>(_getServiceProvider, name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddStep(task);
            InitEvents(block);
            _tasks.Enqueue(block);
            return block;
        }

        protected override Task InnerRunAsync(T args, LogDispatcher log, CancellationToken ct)
        {
            return _innerRun(args, log, ct);
        }

        protected Task InnerParallelRunAsync(T args, LogDispatcher log, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Task.CompletedTask;
            var tasks = Tasks.Select(task => task.RunAsync(args, ct));
            return Task.WhenAll(tasks);
        }

        protected async Task InnerSequentialRunAsync(T args, LogDispatcher log, CancellationToken ct)
        {
            foreach (var task in Tasks)
            {
                if (ct.IsCancellationRequested)
                    break;
                await task.RunAsync(args, ct).ConfigureAwait(false);
            }
        }

        private void InitEvents(ManagedTask<T> task)
        {
            task.Started += FireStarted;
            task.Ended += FireEnded;
            task.MessageLogged += Log.Write;
        }

        private static Func<IServiceProvider> GetServiceProvider(Action<IServiceCollection> configuresServices)
        {
            var services = new ServiceCollection();
            configuresServices?.Invoke(services);
            return () => services.BuildServiceProvider();
        }
    }
}
