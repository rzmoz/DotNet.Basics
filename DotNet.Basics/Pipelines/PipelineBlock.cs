using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class PipelineBlock<T> : IEnumerable<PipelineStep<T>> where T : EventArgs, new()
    {
        private readonly IList<PipelineStep<T>> _steps;

        public string Name { get; private set; }

        public PipelineBlock(string name = null, params PipelineStep<T>[] step)
        {
            Name = name ?? string.Empty;
            _steps = new List<PipelineStep<T>>(step);
        }

        public PipelineBlock<T> AddStep<TStep>() where TStep : PipelineStep<T>
        {
            var step = new LazyBindStep<T, TStep>();
            _steps.Add(step);
            return this;
        }

        public void AddSteps(params Func<T, IPipelineLogger, Task>[] asyncFunc)
        {
            AddSteps(asyncFunc.Select(af => (PipelineStep<T>)(new EagerBindStep<T>(af))).ToArray());
        }

        public void AddSteps(params PipelineStep<T>[] steps)
        {
            foreach (var step in steps)
                _steps.Add(step);
        }

        public PipelineStep<T>[] Steps => _steps.ToArray();

        public IEnumerator<PipelineStep<T>> GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
