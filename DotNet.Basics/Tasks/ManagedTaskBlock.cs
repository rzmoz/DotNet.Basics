using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public class ManagedTaskBlock<T> : ManagedTask<T> where T : class, new()
    {
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _innerRun;

        public ManagedTaskBlock() : this(InvokeStyle.Parallel)
        { }

        public ManagedTaskBlock(InvokeStyle invokeStyle) : base((args, issues, ct) => { })
        {
            _tasks = new ConcurrentQueue<ManagedTask<T>>();
            InvokeStyle = invokeStyle;
            switch (InvokeStyle)
            {
                case InvokeStyle.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case InvokeStyle.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(InvokeStyle)} not supported: {InvokeStyle }");
            }
        }
        public ManagedTaskBlock(string name, InvokeStyle invokeStyle) : base(name, (args, issues, ct) => { })
        {
            _tasks = new ConcurrentQueue<ManagedTask<T>>();
            InvokeStyle = invokeStyle;
            switch (InvokeStyle)
            {
                case InvokeStyle.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case InvokeStyle.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(InvokeStyle)} not supported: {InvokeStyle }");
            }
        }
        public IReadOnlyCollection<ManagedTask<T>> Tasks => _tasks.ToList();
        public InvokeStyle InvokeStyle { get; }

        public ManagedTaskBlock<T> AddTask(Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddTask(mt);
        }
        public ManagedTaskBlock<T> AddTask(string name, Action<T, TaskIssueList, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddTask(mt);
        }
        public ManagedTaskBlock<T> AddTask(Func<T, TaskIssueList, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddTask(mt);
        }
        public ManagedTaskBlock<T> AddTask(string name, Func<T, TaskIssueList, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddTask(mt);
        }

        public ManagedTaskBlock<T> AddTask(ManagedTask<T> mt)
        {
            InitEvents(mt);
            _tasks.Enqueue(mt);
            return this;
        }

        public ManagedTaskBlock<T> AddBlock(params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock("", tasks);
        }
        public ManagedTaskBlock<T> AddBlock(string name, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            return AddBlock(name, InvokeStyle.Parallel, tasks);
        }

        public ManagedTaskBlock<T> AddBlock(string name, InvokeStyle invokeStyle = InvokeStyle.Parallel, params Func<T, TaskIssueList, CancellationToken, Task>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(ManagedTaskBlock<T>));
            var block = new ManagedTaskBlock<T>(name ?? $"Block {count}", invokeStyle);
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
