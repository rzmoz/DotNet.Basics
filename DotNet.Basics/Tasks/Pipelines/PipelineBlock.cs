using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Collections;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class PipelineBlock<T> : PipelineSection<T>, IEnumerable<PipelineSection<T>> where T : class, new()
    {
        private readonly IContainer _container;
        private readonly List<PipelineSection<T>> _subSections;

        private readonly Func<T, TaskIssueList, CancellationToken, Task> _innerRun;

        public PipelineBlock()
            : this(BlockRunType.Parallel)
        { }

        public PipelineBlock(IContainer container)
            : this(null, container, BlockRunType.Parallel)
        { }

        public PipelineBlock(BlockRunType blockRunType)
            : this(null, null, blockRunType)
        { }

        public PipelineBlock(string name, BlockRunType blockRunType)
            : this(name, null, blockRunType)
        { }
        public PipelineBlock(IContainer container, BlockRunType blockRunType)
            : this(null, container, blockRunType)
        { }

        public PipelineBlock(string name, IContainer container, BlockRunType blockRunType)
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
            var lazyStep = new LazyBindSection<T, TStep>(name ?? typeof(TStep).Name, _container.Resolve<TStep>);
            InitEvents(lazyStep);
            _subSections.Add(lazyStep);
            return this;
        }

        public PipelineBlock<T> AddStep(Func<T, TaskIssueList, CancellationToken, Task> step)
        {
            return AddStep(null, step);
        }

        public PipelineBlock<T> AddStep(string name, Func<T, TaskIssueList, CancellationToken, Task> step)
        {
            var eagerStep = new EagerBindStep<T>(name ?? $"{PipelineTaskTypes.Step} {_subSections.Count}", step);
            InitEvents(eagerStep);
            _subSections.Add(eagerStep);
            return this;
        }

        public PipelineBlock<T> AddBlock(string name, params Func<T, TaskIssueList, CancellationToken, Task>[] steps)
        {
            return AddBlock(name, BlockRunType.Parallel, steps);
        }

        public PipelineBlock<T> AddBlock(string name, BlockRunType blockRunType = BlockRunType.Parallel, params Func<T, TaskIssueList, CancellationToken, Task>[] steps)
        {
            var count = _subSections.Count(s => s.TaskType == PipelineTaskTypes.Block);
            var block = new PipelineBlock<T>(name ?? $"Block {count}", _container, blockRunType);
            steps.ForEach(step => block.AddStep(step));
            InitEvents(block);
            _subSections.Add(block);
            return block;
        }

        public override string TaskType => PipelineTaskTypes.Block;

        protected override async Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            await _innerRun(args, issues, ct).ConfigureAwait(false);
        }

        protected async Task InnerParallelRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            DebugOut.WriteLine($"Running block {Name} in parallel");
            if (ct.IsCancellationRequested)
                return;
            await _subSections.ParallelForEachAsync(async s =>
            {
                var result = await s.RunAsync(args, ct).ConfigureAwait(false);
                issues.Add(result.Issues);
            }).ConfigureAwait(false);
        }
        protected async Task InnerSequentialRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            DebugOut.WriteLine($"Running block {Name} in sequence");
            foreach (var section in SubSections)
            {
                if (ct.IsCancellationRequested)
                    break;
                var result = await section.RunAsync(args, ct).ConfigureAwait(false);
                issues.Add(result.Issues);
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