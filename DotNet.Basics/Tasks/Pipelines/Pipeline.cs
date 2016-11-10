using System;
using Autofac;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    {
        public Pipeline()
        {
        }

        public Pipeline(IContainer container) : base(container)
        {
        }

        public Pipeline(BlockRunType blockRunType) : base(blockRunType)
        {
        }

        public Pipeline(string name, BlockRunType blockRunType) : base(name, blockRunType)
        {
        }

        public Pipeline(IContainer container, BlockRunType blockRunType) : base(container, blockRunType)
        {
        }

        public Pipeline(string name, IContainer container, BlockRunType blockRunType) : base(name, container, blockRunType)
        {
        }
    }
    public class Pipeline<T> : PipelineBlock<T> where T : class, new()
    {
        public Pipeline()
        {
        }

        public Pipeline(IContainer container) : base(container)
        {
        }

        public Pipeline(BlockRunType blockRunType) : base(blockRunType)
        {
        }

        public Pipeline(string name, BlockRunType blockRunType) : base(name, blockRunType)
        {
        }

        public Pipeline(IContainer container, BlockRunType blockRunType) : base(container, blockRunType)
        {
        }

        public Pipeline(string name, IContainer container, BlockRunType blockRunType) : base(name, container, blockRunType)
        {
        }

        public override string TaskType => PipelineTaskTypes.Pipeline;
    }
}
