using System;

namespace DotNet.Basics.Pipelines
{
    public class PipelineResult<T> where T : EventArgs, new()
    {
        public PipelineResult(bool success, T args = null)
        {
            Success = success;
            Args = args ?? new T();
        }

        public T Args { get; private set; }
        public bool Success { get; }
    }
}
