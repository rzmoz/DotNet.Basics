using System.Collections.Generic;

namespace DotNet.Basics.Tasks.Pipelines
{
    public interface IPipeline : ITask
    {
        IReadOnlyCollection<ITask> Tasks { get; }
    }
}
