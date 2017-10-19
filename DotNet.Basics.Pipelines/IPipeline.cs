using System.Collections.Generic;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public interface IPipeline : ITask
    {
        IReadOnlyCollection<ITask> Tasks { get; }
    }
}
