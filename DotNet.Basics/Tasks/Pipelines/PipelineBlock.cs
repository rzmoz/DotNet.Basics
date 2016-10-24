using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Collections;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class PipelineBlock<T> : PipelineSection<T>, IEnumerable<PipelineSection<T>> where T : new()
    {
        private readonly IContainer _container;
        private readonly List<PipelineSection<T>> _subSections;

        private readonly Func<T, CancellationToken, Task> _innerRun;

        public PipelineBlock(BlockRunType blockRunType = BlockRunType.Parallel)
            : this(null, null, blockRunType)
        {
        }

        public PipelineBlock(string name = null, BlockRunType blockRunType = BlockRunType.Parallel)
            : this(name, null, blockRunType)
        {
        }
        public PipelineBlock(IContainer container, BlockRunType blockRunType = BlockRunType.Parallel)
            : this(null, container, blockRunType)
        {
        }

        public PipelineBlock(string name, IContainer container, BlockRunType blockRunType = BlockRunType.Parallel)
            : base(name)
        {
            _container = container ?? new IocBuilder().Container;
            _subSections = new List<PipelineSection<T>>();
            RunType = blockRunType;
            switch (RunType)
            {
                case BlockRunType.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case BlockRunType.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(RunType)} not supported: {RunType}");
            }
        }

        protected IReadOnlyCollection<PipelineSection<T>> SubSections => _subSections;
        public BlockRunType RunType { get; }

        public PipelineBlock<T> AddStep<TStep>(string name = null) where TStep : PipelineSection<T>
        {
            var lazyStep = new LazyBindSection<T, TStep>(name, _container.Resolve<TStep>);
            InitEvents(lazyStep);
            _subSections.Add(lazyStep);
            return this;
        }

        public PipelineBlock<T> AddStep(Func<T, CancellationToken, Task> step)
        {
            return AddStep(null, step);
        }

        public PipelineBlock<T> AddStep(string name, Func<T, CancellationToken, Task> step)
        {
            var eagerStep = new EagerBindStep<T>(name ?? $"{PipelineTaskTypes.Step} {_subSections.Count}", step);
            InitEvents(eagerStep);
            _subSections.Add(eagerStep);
            return this;
        }

        public PipelineBlock<T> AddBlock(string name, params Func<T, CancellationToken, Task>[] steps)
        {
            return AddBlock(name, BlockRunType.Parallel, steps);
        }

        public PipelineBlock<T> AddBlock(string name, BlockRunType blockRunType = BlockRunType.Parallel, params Func<T, CancellationToken, Task>[] steps)
        {
            var count = _subSections.Count(s => s.TaskType == PipelineTaskTypes.Block);
            var block = new PipelineBlock<T>(name ?? $"Block {count}", _container, blockRunType);
            steps.ForEach(step => block.AddStep(step));
            InitEvents(block);
            _subSections.Add(block);
            return block;
        }

        public override string TaskType => PipelineTaskTypes.Block;

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            await _innerRun(args, ct).ConfigureAwait(false);
        }
        protected async Task InnerParallelRunAsync(T args, CancellationToken ct)
        {
            DebugOut.WriteLine($"Running block {Name} in parallel");
            await _subSections.ParallelForEachAsync(s => s.RunAsync(args, ct)).ConfigureAwait(false);
        }
        protected async Task InnerSequentialRunAsync(T args, CancellationToken ct)
        {
            DebugOut.WriteLine($"Running block {Name} in sequence");
            foreach (var section in SubSections)
            {
                await section.RunAsync(args, ct).ConfigureAwait(false);
                if (ct.IsCancellationRequested)
                    break;
            }
        }

        private void InitEvents(PipelineSection<T> section)
        {
            section.Started += FireStarted;
            section.Ended += FireEnded;
        }

        public IEnumerator<PipelineSection<T>> GetEnumerator()
        {
            return _subSections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}