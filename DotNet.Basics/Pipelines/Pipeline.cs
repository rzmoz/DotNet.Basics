using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class Pipeline<T> : PipelineBlock<T> where T : EventArgs, new()
    {
        public Pipeline(string name = null) : this(name, null)
        {
        }
        public Pipeline(SimpleContainer container) : this(null, container)
        {
        }
        public Pipeline(string name, SimpleContainer container) : base(name ?? nameof(SectionType.Pipeline), container)
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
