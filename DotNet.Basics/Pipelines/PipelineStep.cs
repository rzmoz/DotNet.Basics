using System;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> : PipelineSection<T> where T : EventArgs, new()
    {
        protected PipelineStep(string name = null) : base(name)
        {
        }

        public override SectionType SectionType => SectionType.Step;
    }
}
