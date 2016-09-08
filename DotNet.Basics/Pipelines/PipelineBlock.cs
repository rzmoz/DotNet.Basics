using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class PipelineBlock<T> : PipelineSection<T>, IEnumerable<PipelineSection<T>> where T : EventArgs,new()
    {
        private readonly SimpleContainer _container;
        private readonly List<PipelineSection<T>> _subSections;

        public PipelineBlock(string name = null, SimpleContainer container = null)
            : base(name)
        {
            _container = container ?? new SimpleContainer();
            _subSections = new List<PipelineSection<T>>();
        }

        protected IReadOnlyCollection<PipelineSection<T>> SubSections => _subSections;

        public PipelineBlock<T> AddStep<TStep>(string name = null) where TStep : PipelineSection<T>
        {
            var lazyStep = new LazyBindStep<T, TStep>(name, _container.GetInstance<TStep>);
            InitEvents(lazyStep);
            _subSections.Add(lazyStep);
            return this;
        }

        public PipelineBlock<T> AddStep(string name, Func<T, CancellationToken, Task> step)
        {
            var eagerStep = new EagerBindStep<T>(name, step);
            InitEvents(eagerStep);
            _subSections.Add(eagerStep);
            return this;
        }
        public PipelineBlock<T> AddBlock(string name)
        {
            var block = new PipelineBlock<T>(name);
            InitEvents(block);
            _subSections.Add(block);
            return block;
        }
        public PipelineBlock<T> AddSections(params PipelineSection<T>[] sections)
        {
            foreach (var section in sections)
                _subSections.Add(section);
            return this;
        }

        public override SectionType SectionType => SectionType.Block;

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            await
                _subSections.ParallelForEachAsync(async section => await section.RunAsync(args, ct).ConfigureAwait(false))
                    .ConfigureAwait(false);
        }

        private void InitEvents(PipelineSection<T> section)
        {
            section.SectionStarted += FireSectionStarted;
            section.SectionEnded += FireSectionEnded;
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
