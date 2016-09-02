using System;

namespace DotNet.Basics.Pipelines
{
    public class PipelineResult<T> where T : EventArgs, new()
    {
        public PipelineResult(Exception lastException = null, T args = null)
        {
            LastException = lastException;
            Args = args ?? new T();
        }

        public T Args { get; private set; }
        public Exception LastException { get; }
    }
}
