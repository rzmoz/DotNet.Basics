using System;

namespace DotNet.Basics.Tasks.Pipelines
{
    public interface ILazyLoadStep : ITask
    {
        object GetTask();
        Type GetTaskType();
    }
}
