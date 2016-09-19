using System;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> : PipelineSection<T> where T : EventArgs, new()
    {
        protected PipelineStep(string name) : base(name)
        {
        }

        public override SectionType SectionType => SectionType.Step;
    }
}
