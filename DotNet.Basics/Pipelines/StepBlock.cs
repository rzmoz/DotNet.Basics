using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class StepBlock<T> : IEnumerable<TaskStep<T>> where T : EventArgs, new()
    {
        private readonly IList<TaskStep<T>> _steps;

        public string Name { get; private set; }

        public StepBlock(string name = null, params TaskStep<T>[] step)
        {
            Name = name ?? string.Empty;
            _steps = new List<TaskStep<T>>(step);
        }

        public StepBlock<T> AddStep<TStep>() where TStep : TaskStep<T>
        {
            var step = new LazyBindStep<T, TStep>();
            _steps.Add(step);
            return this;
        }

        public void AddSteps(params Func<T, IDiagnostics, Task>[] asyncFunc)
        {
            AddSteps(asyncFunc.Select(af => (TaskStep<T>)(new EagerBindStep<T>(af))).ToArray());
        }

        public void AddSteps(params TaskStep<T>[] steps)
        {
            foreach (var step in steps)
                _steps.Add(step);
        }

        public TaskStep<T>[] Steps => _steps.ToArray();

        public IEnumerator<TaskStep<T>> GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
