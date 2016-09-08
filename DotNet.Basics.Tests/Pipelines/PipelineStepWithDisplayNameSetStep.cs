using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineSection<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep(string name) : base(name)
        {
        }

        public override SectionType SectionType => SectionType.Step;
        protected override async Task InnerRunAsync(EventArgs args, CancellationToken ct)
        {
            await Task.Delay(1.MilliSeconds(), ct).ConfigureAwait(false);//silence compiler warning
            Console.WriteLine($"Display name set to {Name}");
        }
    }
}
