using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    {
        public Pipeline()
        { }

        public Pipeline(string name) : base(name)
        { }
        
        public Pipeline(Invoke invoke) : base(invoke)
        { }

        public Pipeline(string name, Invoke invoke) : base(name, invoke)
        { }
    }
    public class Pipeline<T> : ManagedTask<T> where T : class, new()
    {
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _innerRun;

        public Pipeline() : this(Invoke.Sequential)
        { }

        public Pipeline(string name) : this(name, Invoke.Sequential)
        { }

        public Pipeline(Invoke invoke)
            : this(null, invoke)
        { }

        public Pipeline(string name, Invoke invoke)
            : base(name)
        {
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

        public TaskResult AssertLazyLoadSteps()
        {
            return AssertLazyLoadSteps(Tasks);
        }

        private TaskResult AssertLazyLoadSteps(IReadOnlyCollection<ITask> tasks)
        {
            return new TaskResult(issues =>
            {
                foreach (var pipeline in tasks.OfType<Pipeline>())
                    issues.AddRange(AssertLazyLoadSteps(pipeline.Tasks).Issues);
                foreach (var lazyLoadStep in tasks.OfType<ILazyLoadStep>())
                {
                    try
                    {
                        lazyLoadStep.GetTask();
                    }
                    catch (InvalidOperationException e)
                    {
                        issues.Add($"Failed to load: {lazyLoadStep.GetTaskType().Name} - {e.Message}", e);
                    }
                }
            });
        }

        public Pipeline<T> AddStep<TTask>(IServiceProvider serviceProvider,string name = null) where TTask : ManagedTask<T>
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            var lazyTask = new LazyLoadStep<T, TTask>(name, ()=> serviceProvider.GetService(typeof(TTask)) as TTask);
            InitEvents(lazyTask);
            _tasks.Enqueue(lazyTask);
            return this;
        }

        public Pipeline<T> AddStep(Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(Func<T, TaskIssueList, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Func<T, TaskIssueList, CancellationToken, Task> task)
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

        public Pipeline<T> AddBlock(params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock(null, tasks);
        }

        public Pipeline<T> AddBlock(string name, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public Pipeline<T> AddBlock(string name, Invoke invoke = Invoke.Parallel, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(Pipeline<>));
            var block = new Pipeline<T>(name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddStep(task);
            InitEvents(block);
            _tasks.Enqueue(block);
            return block;
        }

        protected override async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            await _innerRun(args, issues, ct).ConfigureAwait(false);
        }

        protected async Task InnerParallelRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return;
            var tasks = Tasks.Select(async t =>
            {
                var result = await t.RunAsync(args, ct).ConfigureAwait(false);
                issues.AddRange(result.Issues);

            }).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        protected async Task InnerSequentialRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            foreach (var task in Tasks)
            {
                if (ct.IsCancellationRequested)
                    break;
                var result = await task.RunAsync(args, ct).ConfigureAwait(false);
                issues.AddRange(result.Issues);
            }
        }

        private void InitEvents(ManagedTask<T> section)
        {
            section.Started += FireStarted;
            section.Ended += FireEnded;
        }
    }
}
