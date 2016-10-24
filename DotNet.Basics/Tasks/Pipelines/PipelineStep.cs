namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : PipelineSection<T> where T : new()
    {
        protected PipelineStep() : this(null)
        {
        }
        protected PipelineStep(string name) : base(name)
        {
        }

        public override string TaskType => PipelineTaskTypes.Step;
    }
}
