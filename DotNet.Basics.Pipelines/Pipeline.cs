﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class Pipeline<T> : ManagedTask<T>
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentQueue<ManagedTask<T>> _tasks = new();
        private readonly Func<T, Task<int>> _innerRun;

        public Pipeline(IServiceProvider serviceProvider, string? name = null, Invoke invoke = Invoke.Sequential)
            : base(name)
        {
            _serviceProvider = serviceProvider;
            switch (invoke)
            {
                case Invoke.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case Invoke.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(Invoke)} not supported: {invoke}");
            }
        }

        public IReadOnlyCollection<ManagedTask<T>> Tasks => _tasks.ToList();
        
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

        public Pipeline<T> AddStep<TTask>(string? name = null) where TTask : ManagedTask<T>
        {
            var lazyTask = new LazyLoadStep<T, TTask>(name, _serviceProvider);
            InitEvents(lazyTask);
            _tasks.Enqueue(lazyTask);
            return this;
        }

        public Pipeline<T> AddStep(Action<T> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Action<T> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(Func<T, Task<int>> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Func<T, Task<int>> task)
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

        public Pipeline<T> AddBlock(params Func<T, Task<int>>[] tasks)
        {
            return AddBlock(null, tasks);
        }

        public Pipeline<T> AddBlock(string? name, params Func<T, Task<int>>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public Pipeline<T> AddBlock(string? name, Invoke invoke = Invoke.Parallel, params Func<T, Task<int>>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(Pipeline<>));
            var block = new Pipeline<T>(_serviceProvider, name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddStep(task);
            InitEvents(block);
            _tasks.Enqueue(block);
            return block;
        }

        protected override async Task<int> InnerRunAsync(T args)
        {
            return await _innerRun(args);
        }

        protected async Task<int> InnerParallelRunAsync(T args)
        {
            var tasks = Tasks.Select(task => task.RunAsync(args));
            var results = await Task.WhenAll(tasks);
            return results.Select(i => i < 0 ? i * -1 : i).Sum();
        }

        protected async Task<int> InnerSequentialRunAsync(T args)
        {
            foreach (var task in Tasks)
            {
                var result = await task.RunAsync(args);
                if (result != 0)
                    return result;
            }

            return 0;
        }

        private void InitEvents(ManagedTask<T> task)
        {
            task.Started += FireStarted;
            task.Ended += FireEnded;
        }
    }
}
