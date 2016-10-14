using System;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : PipelineSection<T> where T : EventArgs, new()
    {
        protected PipelineStep(string name) : base(name)
        {
        }

        public override string TaskType => PipelineTaskTypes.Step;
    }
}
