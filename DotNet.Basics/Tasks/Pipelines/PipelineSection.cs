using System;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineSection<T> : ManagedTask<T> where T : EventArgs, new()
    {
        protected PipelineSection(string name) : base((args, ct) => { })
        {
            Name = name;
        }
    }
}
