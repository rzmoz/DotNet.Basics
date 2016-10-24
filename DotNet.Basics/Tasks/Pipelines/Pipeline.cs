using System;
using System.Threading;
using System.Threading.Tasks;
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
        public Pipeline(string name = null) : base(name)
        {
        }

        public Pipeline(IContainer container) : base(container)
        {
        }

        public Pipeline(string name, IContainer container) : base(name, container)
        {
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            foreach (var section in SubSections)
            {
                await section.RunAsync(args, ct).ConfigureAwait(false);
                if (ct.IsCancellationRequested)
                    break;
            }
        }
        public override string TaskType => PipelineTaskTypes.Pipeline;
    }
}
