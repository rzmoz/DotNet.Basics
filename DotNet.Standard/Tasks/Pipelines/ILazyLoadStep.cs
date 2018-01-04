using System;

namespace DotNet.Standard.Tasks.Pipelines
{
    public interface ILazyLoadStep : ITask
    {
        object GetTask();
        Type GetTaskType();
    }
}
