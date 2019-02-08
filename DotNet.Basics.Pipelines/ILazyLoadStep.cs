using System;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public interface ILazyLoadStep : ITask
    {
        object GetTask();
        Type GetTaskType();
    }
}
