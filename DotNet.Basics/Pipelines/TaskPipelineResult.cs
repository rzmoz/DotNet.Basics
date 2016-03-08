using System;

namespace DotNet.Basics.Pipelines
{
    public class TaskPipelineResult<T> where T : EventArgs, new()
    {
        public TaskPipelineResult(T args = null, TaskPipelineProfile pipelineProfile = null)
        {
            Args = args ?? new T();
            Profile = pipelineProfile ?? new TaskPipelineProfile(string.Empty);
        }

        public T Args { get; private set; }
        public TaskPipelineProfile Profile { get; private set; }
    }
}
