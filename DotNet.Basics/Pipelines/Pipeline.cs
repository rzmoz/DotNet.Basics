using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    {
    }

    public class Pipeline<T> : PipelineBlock<T> where T : EventArgs, new()
    {
        public Pipeline(string name = null, SimpleContainer container = null) : base(name, container)
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
        public override SectionType SectionType => SectionType.Pipeline;
    }
}
