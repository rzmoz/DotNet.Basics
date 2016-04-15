using System;

namespace DotNet.Basics.Pipelines
{
    public class PipelineResult<T> where T : EventArgs, new()
    {
        public PipelineResult(T args = null)
        {
            Args = args ?? new T();
        }

        public T Args { get; private set; }
    }
}
