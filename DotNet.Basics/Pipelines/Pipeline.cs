using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    { }

    public class Pipeline<T> : IEnumerable<PipelineBlock<T>> where T : EventArgs, new()
    {
        private readonly IList<PipelineBlock<T>> _stepBlocks;

        public Pipeline()
        {
            _stepBlocks = new List<PipelineBlock<T>>();
        }

        public PipelineBlock<T>[] PipelineBlocks => _stepBlocks.ToArray();

        public PipelineBlock<T> AddBlock(string blockName = null)
        {
            var block = CreateStepBlock(blockName);
            return block;
        }
        public PipelineBlock<T> AddBlock(string blockName, params Func<T, IDiagnostics, Task>[] asyncFunc)
        {
            var block = CreateStepBlock(blockName);
            block.AddSteps(asyncFunc);
            return block;
        }
        public PipelineBlock<T> AddBlock(params Func<T, IDiagnostics, Task>[] asyncFunc)
        {
            return AddBlock(null, asyncFunc);
        }

        public PipelineBlock<T> AddBlock(params PipelineStep<T>[] steps)
        {
            var block = CreateStepBlock();
            block.AddSteps(steps);
            return block;
        }

        private PipelineBlock<T> CreateStepBlock(string blockName = null)
        {
            var stepBlock = new PipelineBlock<T>(blockName);
            _stepBlocks.Add(stepBlock);
            return stepBlock;
        }

        public IEnumerator<PipelineBlock<T>> GetEnumerator()
        {
            return _stepBlocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
