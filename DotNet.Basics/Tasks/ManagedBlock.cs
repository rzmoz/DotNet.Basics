using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Collections;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tasks
{
    public class ManagedBlock : ManagedBlock<EventArgs>
    {
        public ManagedBlock()
        { }

        public ManagedBlock(string name) : base(name)
        { }

        public ManagedBlock(IContainer container) : base(container)
        { }

        public ManagedBlock(Invoke invoke) : base(invoke)
        { }

        public ManagedBlock(string name, Invoke invoke) : base(name, invoke)
        { }

        public ManagedBlock(IContainer container, Invoke invoke) : base(container, invoke)
        { }

        public ManagedBlock(string name, IContainer container, Invoke invoke) : base(name, container, invoke)
        { }
    }
    public class ManagedBlock<T> : ManagedTask<T> where T : class, new()
    {
        private readonly IContainer _container;
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _innerRun;

        public ManagedBlock() : this(Invoke.Parallel)
        { }

        public ManagedBlock(string name) : this(name, Invoke.Parallel)
        { }

        public ManagedBlock(IContainer container)
            : this(null, container, Invoke.Parallel)
        { }

        public ManagedBlock(Invoke invoke)
            : this(null, null, invoke)
        { }

        public ManagedBlock(string name, Invoke invoke)
            : this(name, null, invoke)
        { }
        public ManagedBlock(IContainer container, Invoke invoke)
            : this(null, container, invoke)
        { }

        public ManagedBlock(string name, IContainer container, Invoke invoke)
            : base(name)
        {
            _container = container ?? new IocBuilder().Container;
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

        public ManagedBlock<T> AddTask<TTask>(string name = null) where TTask : ManagedTask<T>
        {
            var lazyTask = new LazyLoadManagedTask<T, TTask>(name, _container.Resolve<TTask>);
            InitEvents(lazyTask);
            _tasks.Enqueue(lazyTask);
            return this;
        }

        public ManagedBlock<T> AddTask(Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddTask(mt);
        }
        public ManagedBlock<T> AddTask(string name, Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddTask(mt);
        }
        public ManagedBlock<T> AddTask(Func<T, TaskIssueList, CancellationToken, System.Threading.Tasks.Task> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddTask(mt);
        }
        public ManagedBlock<T> AddTask(string name, Func<T, TaskIssueList, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddTask(mt);
        }

        public ManagedBlock<T> AddTask(ManagedTask<T> mt)
        {
            InitEvents(mt);
            _tasks.Enqueue(mt);
            return this;
        }

        public ManagedBlock<T> AddBlock(params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock(null, tasks);
        }
        public ManagedBlock<T> AddBlock(string name, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public ManagedBlock<T> AddBlock(string name, Invoke invoke = Invoke.Parallel, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(ManagedBlock<T>));
            var block = new ManagedBlock<T>(name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddTask(task);
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
            DebugOut.WriteLine($"Running block {Name} in parallel");
            if (ct.IsCancellationRequested)
                return;
            await Tasks.ParallelForEachAsync(async s =>
            {
                var result = await s.RunAsync(args, ct).ConfigureAwait(false);
                issues.Add(result.Issues);
            }).ConfigureAwait(false);
        }
        protected async Task InnerSequentialRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            DebugOut.WriteLine($"Running block {Name} in sequence");
            foreach (var task in Tasks)
            {
                if (ct.IsCancellationRequested)
                    break;
                var result = await task.RunAsync(args, ct).ConfigureAwait(false);
                issues.Add(result.Issues);
            }
        }

        private void InitEvents(ManagedTask<T> section)
        {
            section.Started += FireStarted;
            section.Ended += FireEnded;
        }
    }
}
