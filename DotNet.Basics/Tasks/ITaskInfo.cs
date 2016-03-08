using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public interface ITaskInfo : ITaskId
    {
        KeyValueCollection Metadata { get; }
    }
}
