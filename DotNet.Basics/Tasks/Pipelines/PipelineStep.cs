namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : PipelineSection<T> where T : new()
    {
        protected PipelineStep(string name = null) : base(name)
        {
        }

        public override string TaskType => PipelineTaskTypes.Step;
    }
}
