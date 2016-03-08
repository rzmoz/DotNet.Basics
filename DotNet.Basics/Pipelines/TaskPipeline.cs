using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class TaskPipeline<T> : IEnumerable<StepBlock<T>> where T : EventArgs, new()
    {
        private readonly IList<StepBlock<T>> _stepBlocks;

        public TaskPipeline()
        {
            _stepBlocks = new List<StepBlock<T>>();
        }

        public StepBlock<T>[] StepBlocks => _stepBlocks.ToArray();

        public StepBlock<T> AddBlock(string blockName = null)
        {
            var block = CreateStepBlock(blockName);
            return block;
        }
        public StepBlock<T> AddBlock(string blockName, params Func<T, IDiagnostics, Task>[] asyncFunc)
        {
            var block = CreateStepBlock(blockName);
            block.AddSteps(asyncFunc);
            return block;
        }
        public StepBlock<T> AddBlock(params Func<T, IDiagnostics, Task>[] asyncFunc)
        {
            return AddBlock(null, asyncFunc);
        }
        public StepBlock<T> AddBlock(params TaskStep<T>[] steps)
        {
            var block = CreateStepBlock();
            block.AddSteps(steps);
            return block;
        }

        private StepBlock<T> CreateStepBlock(string blockName = null)
        {
            var stepBlock = new StepBlock<T>(blockName);
            _stepBlocks.Add(stepBlock);
            return stepBlock;
        }

        public IEnumerator<StepBlock<T>> GetEnumerator()
        {
            return _stepBlocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
