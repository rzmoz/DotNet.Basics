using System;
using Autofac;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    {
        public Pipeline(string name = null) : base(name)
        {
        }

        public Pipeline(IContainer container) : base(container)
        {
        }

        public Pipeline(string name, IContainer container) : base(name, container)
        {
        }
    }
    public class Pipeline<T> : PipelineBlock<T> where T : new()
    {
        public Pipeline(string name = null) : this(name, null)
        {
        }

        public Pipeline(IContainer container) : this(null, container)
        {
        }

        public Pipeline(string name, IContainer container) : base(name, container, BlockRunType.Sequential)
        {
        }
        public override string TaskType => PipelineTaskTypes.Pipeline;
    }
}
