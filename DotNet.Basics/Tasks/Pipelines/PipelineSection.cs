using System;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineSection<T> : ManagedTask<T> where T : EventArgs, new()
    {
        protected PipelineSection()
            : this(null)
        {
        }
        protected PipelineSection(string name) : base((args, ct) => { })
        {
            Name = ResolveName(name);
        }

        private string ResolveName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var type = GetType();
                if (type == typeof(PipelineStep<T>))
                    return PipelineTaskTypes.Step;
                if (type == typeof(PipelineBlock<T>))
                    return PipelineTaskTypes.Block;
                if (type == typeof(Pipeline<T>))
                    return PipelineTaskTypes.Pipeline;

                return null;
            }
            return name;
        }
    }
}
